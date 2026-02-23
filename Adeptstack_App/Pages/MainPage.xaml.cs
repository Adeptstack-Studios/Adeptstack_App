using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;

namespace Adeptstack_App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        this.Title = "Adeptstack News";
        NewsRefresh();
    }

    void NewsRefresh()
    {
        Thread newsThread = new Thread(delegate ()
        {
            Dispatcher.Dispatch(() =>
            {
                refresh.IsEnabled = false;
                loading.IsVisible = true;
                busy.IsRunning = true;
                refresh.IsRefreshing = false;
            });
            RefreshingNews();
        });
        newsThread.Start();
    }

    async void RefreshingNews()
    {
        bool isConnected = await Web.IsConnectedToInternetAsync();
        if (isConnected) Dispatcher.Dispatch(() => internet.IsVisible = false);
        if (!isConnected) Dispatcher.Dispatch(() => internet.IsVisible = true);
        List<NewsContext> news = Web.GetNews();
        Dispatcher.Dispatch(() => newsLayout.Children.Clear());

        if (news.Count > 0)
        {
            Dispatcher.Dispatch(() => nothing.IsVisible = false);
            foreach (NewsContext newsItem in news)
            {
                NewsView newsView = new NewsView
                {
                    News = newsItem,
                };
                newsView.NewsClicked += News_Clicked;

                Dispatcher.Dispatch(() => newsLayout.Children.Add(newsView));
            }
        }
        else
        {
            Dispatcher.Dispatch(() => nothing.IsVisible = true);
        }

        Dispatcher.Dispatch(() =>
        {
            refresh.IsEnabled = true;
            loading.IsVisible = false;
            busy.IsRunning = false;
        });
    }

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
        NewsRefresh();
    }

    private void News_Clicked(object sender, NewsContext e)
    {
        Navigation.PushAsync(new DisplayContent(e));
    }
}

