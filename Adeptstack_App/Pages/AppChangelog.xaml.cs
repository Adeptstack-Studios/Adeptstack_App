using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Utils;
using System.Runtime.CompilerServices;

namespace Adeptstack_App;

public partial class AppChangelog : ContentPage
{
    string app = "app";

    public AppChangelog(string app)
    {
        InitializeComponent();
        this.app = app;
        this.Title = $"{app} Changelogs";
        ChangelogsRefresh();
    }

    void ChangelogsRefresh()
    {
        Thread changelogsThread = new Thread(delegate ()
        {
            Dispatcher.Dispatch(() =>
            {
                refresh.IsEnabled = false;
                loading.IsVisible = true;
                busy.IsRunning = true;
                refresh.IsRefreshing = false;
            });
            RefreshingChangelogs();
        });
        changelogsThread.Start();
    }

    async void RefreshingChangelogs()
    {
        bool isConnected = await Web.IsConnectedToInternetAsync();
        if (isConnected) Dispatcher.Dispatch(() => internet.IsVisible = false);
        if (!isConnected) Dispatcher.Dispatch(() => internet.IsVisible = true);
        List<ChangelogContext> changelogs = Web.GetChangelogs(this.app);
        Dispatcher.Dispatch(() => changelogsLayout.Children.Clear());

        if (changelogs.Count > 0)
        {
            Dispatcher.Dispatch(() => nothing.IsVisible = false);
            foreach (ChangelogContext changelogItem in changelogs)
            {
                ChangelogView changelogView = new ChangelogView
                {
                    Changelog = changelogItem,
                };
                changelogView.ChangelogClicked += Changelog_Clicked;

                Dispatcher.Dispatch(() => changelogsLayout.Children.Add(changelogView));
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
        ChangelogsRefresh();
    }

    private void Changelog_Clicked(object sender, ChangelogContext e)
    {
        Navigation.PushAsync(new DisplayContent(e));
    }
}