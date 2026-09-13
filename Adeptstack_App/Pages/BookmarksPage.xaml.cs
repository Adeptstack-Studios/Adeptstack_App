using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Utils;

namespace Adeptstack_App.Pages;

public partial class BookmarksPage : ContentPage
{
    public BookmarksPage()
    {
        InitializeComponent();
    }

    // Beim Zurückkehren aus DisplayContent kann ein Lesezeichen entfernt oder hinzugefügt worden sein.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        RenderBookmarks();
    }

    private void RenderBookmarks()
    {
        List<NewsContext> news = Bookmarks.GetNews();
        List<ChangelogContext> changelogs = Bookmarks.GetChangelogs();

        newsLayout.Children.Clear();
        changelogsLayout.Children.Clear();

        foreach (NewsContext newsItem in news)
        {
            NewsView newsView = new NewsView { News = newsItem };
            newsView.NewsClicked += News_Clicked;
            newsLayout.Children.Add(newsView);
        }

        foreach (ChangelogContext changelogItem in changelogs)
        {
            ChangelogView changelogView = new ChangelogView
            {
                Changelog = changelogItem,
                Margin = new Thickness(8, 8, 8, 16)
            };
            changelogView.ChangelogClicked += Changelog_Clicked;
            changelogsLayout.Children.Add(changelogView);
        }

        newsHeader.IsVisible = news.Count > 0;
        changelogsHeader.IsVisible = changelogs.Count > 0;
        nothing.IsVisible = news.Count == 0 && changelogs.Count == 0;
    }

    private void News_Clicked(object sender, NewsContext e)
    {
        Navigation.PushAsync(new DisplayContent(e));
    }

    private void Changelog_Clicked(object sender, ChangelogContext e)
    {
        Navigation.PushAsync(new DisplayContent(e));
    }
}
