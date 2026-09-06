using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Utils;
using Markdig;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;

namespace Adeptstack_App;

public partial class DisplayContent : ContentPage
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public DisplayContent(NewsContext news)
    {
        InitializeComponent();
        this.Title = news.title;

        LoadContentAsync(news.content, news.imageUrl, news.title, news.category, news.publishedAt, news.description);
    }

    public DisplayContent(ChangelogContext changelog)
    {
        InitializeComponent();
        this.Title = changelog.title;

        LoadChangelogDataAsync(changelog);
    }

    private async void LoadChangelogDataAsync(ChangelogContext changelog)
    {
        string appName = "CHANGELOG";

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

    private async void web_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url != "file:///android_asset/" && !e.Url.Contains("data:text/html") && !e.Url.StartsWith("about:blank"))
        {
            e.Cancel = true;
            await Browser.Default.OpenAsync(e.Url, BrowserLaunchMode.SystemPreferred);
        }
    }
}