using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Utils;
using Markdig;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;
using AppContext = Adeptstack_App.ContextClasses.AppContext;

namespace Adeptstack_App;

public partial class DisplayContent : ContentPage
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private string _shareTitle;
    private string _shareUrl;
    private Func<bool> _isBookmarked;
    private Func<bool> _toggleBookmark;

    public DisplayContent(NewsContext news)
    {
        InitializeComponent();
        this.Title = news.title;

        _shareTitle = news.title;
        _shareUrl = Utilities.GetNewsUrl(news);
        _isBookmarked = () => Bookmarks.IsBookmarked(news);
        _toggleBookmark = () => Bookmarks.Toggle(news);
        UpdateBookmarkItem();

        LoadContentAsync(news.content, news.imageUrl, news.title, news.category, news.publishedAt, news.description);
    }

    /// <param name="app">Optional: ist die App schon bekannt, spart das den zusätzlichen API-Call für Name und Slug.</param>
    public DisplayContent(ChangelogContext changelog, AppContext app = null)
    {
        InitializeComponent();
        this.Title = changelog.title;

        _shareTitle = changelog.title;
        _shareUrl = Utilities.GetChangelogUrl(app?.slug);
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
                        _shareUrl = Utilities.GetChangelogUrl(appData.slug);
                    }
                }
                catch
                {
                }
            });
        }

        LoadContentAsync(changelog.content, changelog.imageUrl, changelog.title, appName, changelog.publishedAt, changelog.description);
    }

    public async void LoadContentAsync(string content, string imgUrl, string title, string category, DateTime date, string description)
    {
        bool isConnected = await Web.IsConnectedToInternetAsync();

        Dispatcher.Dispatch(() =>
        {
            internet.IsVisible = !isConnected;

            if (!string.IsNullOrEmpty(content))
            {
                nothing.IsVisible = false;

                string headerHtml = $@"
                                <div style='margin-bottom: 40px; margin-top: 48px;'>
                                    <div style='display: flex; justify-content: space-between; font-size: 13px; font-weight: bold; margin-bottom: 16px;'>
                                        <span style='color: #3b82f6; text-transform: uppercase; letter-spacing: 1px;'>{category}</span>
                                        <span style='color: #94a3b8;'>{date:MMM dd, yyyy}</span>
                                    </div>
                                    <h1 style='color: #f8fafc; font-size: 26px; line-height: 1.3; margin-top: 0; margin-bottom: 16px;'>{title}</h1>
                                    {(string.IsNullOrEmpty(description) ? "" : $"<p style='color: #94a3b8; font-size: 16px; line-height: 1.5; margin-top: 0; margin-bottom: 32px;'>{description}</p>")}
                                    {(string.IsNullOrEmpty(imgUrl) ? "" : $"<img src='{imgUrl}' style='width: 100%; border-radius: 12px; margin-top: 8px;' />")}
                                </div>";

                string markdownBody = Markdig.Markdown.ToHtml(content, MarkdownPipeline);

                string fullBody = headerHtml + markdownBody;

                string css = MarkdownStyle.CSS();
                string html = MarkdownStyle.GetFullHTML(css, fullBody);

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
        await Utilities.ShareAsync(_shareTitle, _shareUrl);
    }

    private async void web_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url != "file:///android_asset/" && !e.Url.Contains("data:text/html") && !e.Url.StartsWith("about:blank"))
        {
            e.Cancel = true;
            await Browser.Default.OpenAsync(e.Url, BrowserLaunchMode.SystemPreferred);
        }
    }
}
