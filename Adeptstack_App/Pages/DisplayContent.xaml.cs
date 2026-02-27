using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Utils;
using Microsoft.Maui.ApplicationModel;

namespace Adeptstack_App;

public partial class DisplayContent : ContentPage
{
    public DisplayContent(NewsContext news)
    {
        InitializeComponent();
        this.Title = news.title;
        LoadContent(news.content, news.imageUrl);

    }

    public DisplayContent(ChangelogContext changelog)
    {
        InitializeComponent();
        this.Title = changelog.title;
        LoadContent(changelog.content, changelog.imageUrl);

    }

    public async void LoadContent(string content, string imgUrl)
    {
        bool isConnected = await Web.IsConnectedToInternetAsync();
        if (isConnected) Dispatcher.Dispatch(() => internet.IsVisible = false);
        if (!isConnected) Dispatcher.Dispatch(() => internet.IsVisible = true);

        if (!string.IsNullOrEmpty(content))
        {
            Dispatcher.Dispatch(() => nothing.IsVisible = false);
            string css = MarkdownStyle.CSS();
            string body = Markdig.Markdown.ToHtml(content);
            string html = MarkdownStyle.GetFullHTML(css, body, imgUrl);

            web.Navigating += web_Navigating;

            web.Source = new HtmlWebViewSource
            {
                Html = html
            }; 
        }
        else
        {
            Dispatcher.Dispatch(() => nothing.IsVisible = true);
        }
    }

    private void web_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url != "file:///android_asset/" && !e.Url.Contains("data:text/html"))
        {
            e.Cancel = true;
            Browser.Default.OpenAsync(e.Url).Wait();
        }
    }
}