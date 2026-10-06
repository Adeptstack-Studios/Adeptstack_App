using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Utils;
using Markdig;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AppContext = Adeptstack_App.ContextClasses.AppContext;

namespace Adeptstack_App;

public partial class DisplayContent : ContentPage
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    // Im Datensparmodus fliegen die <img>-Tags raus, sonst lädt das WebView sie trotzdem
    private static readonly Regex ImageTag = new Regex("<img[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private const string DataSaverPlaceholder =
        "<p style='color: #64748b; font-size: 0.8125rem; font-style: italic;'>Image hidden (Data Saver)</p>";

    private string _shareTitle;
    private Func<string> _getShareText;
    private Func<bool> _isBookmarked;
    private Func<bool> _toggleBookmark;
    private Task _transition;

    public DisplayContent(NewsContext news)
    {
        InitializeComponent();
        _transition = PageTransition.WaitAsync(this);
        this.Title = news.title;

        _shareTitle = news.title;
        _getShareText = () => Utilities.GetNewsShareText(news);
        _isBookmarked = () => Bookmarks.IsBookmarked(news);
        _toggleBookmark = () => Bookmarks.Toggle(news);
        UpdateBookmarkItem();

        LoadContentAsync(news.content, news.imageUrl, news.title, news.category, news.publishedAt, news.description, news.ReadingTimeText);
    }

    /// <param name="app">Optional: ist die App schon bekannt, spart das den zusätzlichen API-Call für den Namen.</param>
    public DisplayContent(ChangelogContext changelog, AppContext app = null)
    {
        InitializeComponent();
        _transition = PageTransition.WaitAsync(this);
        this.Title = changelog.title;

        _shareTitle = changelog.title;
        _getShareText = () => Utilities.GetChangelogShareText(changelog);
        _isBookmarked = () => Bookmarks.IsBookmarked(changelog);
        _toggleBookmark = () => Bookmarks.Toggle(changelog);
        UpdateBookmarkItem();

        LoadChangelogDataAsync(changelog, app);
    }

    private async void LoadChangelogDataAsync(ChangelogContext changelog, AppContext app)
    {
        string appName = string.IsNullOrEmpty(app?.name) ? "CHANGELOG" : app.name;

        if (app == null)
        {
            await Task.Run(() =>
            {
            try
            {
                var appData = Web.GetAppById(changelog.appId);
                if (appData != null && !string.IsNullOrEmpty(appData.name))
                {
                    appName = appData.name;
                }
            }
            catch
            {
            }
            });
        }

        LoadContentAsync(changelog.content, changelog.imageUrl, changelog.title, appName, changelog.publishedAt, changelog.description);
    }

    /// <param name="dateSuffix">Optional hinter dem Datum, z. B. "5 min read".</param>
    public async void LoadContentAsync(string content, string imgUrl, string title, string category, DateTime date, string description, string dateSuffix = null)
    {
        var connected = Web.IsConnectedToInternetAsync();
        bool loadImages = AppSettings.ShouldLoadImages;
        int textSize = AppSettings.TextSizePercent;

        // Markdown im Hintergrund umwandeln, während die Seite noch hereinfährt
        string html = string.IsNullOrEmpty(content) ? null : await Task.Run(() =>
        {
            // Größen in rem, damit die Schriftgröße aus den Einstellungen alles mitskaliert
            string headerHtml = $@"
                            <div style='margin-bottom: 40px; margin-top: 48px;'>
                                <div style='display: flex; justify-content: space-between; font-size: 0.8125rem; font-weight: bold; margin-bottom: 16px;'>
                                    <span style='color: #3b82f6; text-transform: uppercase; letter-spacing: 1px;'>{category}</span>
                                    <span style='color: #94a3b8;'>{date:MMM dd, yyyy}{(string.IsNullOrEmpty(dateSuffix) ? "" : $" · {dateSuffix}")}</span>
                                </div>
                                <h1 style='color: #f8fafc; font-size: 1.625rem; line-height: 1.3; margin-top: 0; margin-bottom: 16px;'>{title}</h1>
                                {(string.IsNullOrEmpty(description) ? "" : $"<p style='color: #94a3b8; font-size: 1rem; line-height: 1.5; margin-top: 0; margin-bottom: 32px;'>{description}</p>")}
                                {(string.IsNullOrEmpty(imgUrl) || !loadImages ? "" : $"<img src='{imgUrl}' style='width: 100%; border-radius: 12px; margin-top: 8px;' />")}
                            </div>";

            string markdownBody = Markdig.Markdown.ToHtml(content, MarkdownPipeline);

            if (!loadImages)
            {
                markdownBody = ImageTag.Replace(markdownBody, DataSaverPlaceholder);
            }

            string fullBody = headerHtml + markdownBody;

            string css = MarkdownStyle.CSS() + $"html {{ font-size: {textSize}%; }}";
            return MarkdownStyle.GetFullHTML(css, fullBody);
        });

        // Das WebView erst nach der Einschub-Animation befüllen
        await _transition;
        bool isConnected = await connected;

        Dispatcher.Dispatch(() =>
        {
            internet.IsVisible = !isConnected;

            if (html != null)
            {
                nothing.IsVisible = false;

                web.Source = new HtmlWebViewSource
                {
                    Html = html
                };
            }
            else
            {
                nothing.IsVisible = true;
            }
        });
    }

    private void UpdateBookmarkItem()
    {
        bool bookmarked = _isBookmarked();
        bookmarkItem.IconImageSource = bookmarked ? "bookmark_filled.png" : "bookmark.png";
        bookmarkItem.Text = bookmarked ? "Remove Bookmark" : "Bookmark";
    }

    private void Bookmark_Clicked(object sender, EventArgs e)
    {
        _toggleBookmark();
        UpdateBookmarkItem();
    }

    private async void Share_Clicked(object sender, EventArgs e)
    {
        await Utilities.ShareAsync(_shareTitle, _getShareText());
    }

    private async void web_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url != "file:///android_asset/" && !e.Url.Contains("data:text/html") && !e.Url.StartsWith("about:blank"))
        {
            e.Cancel = true;
            await AppSettings.OpenLinkAsync(e.Url);
        }
    }
}
