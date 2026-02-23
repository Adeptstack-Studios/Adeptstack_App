using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Utils;
using System.Runtime.CompilerServices;

namespace Adeptstack_App;

public partial class Changelogs : ContentPage
{
    public Changelogs()
    {
        InitializeComponent();
        this.Title = "Adeptstack Changelogs";
        AppsRefresh();
    }

    void AppsRefresh()
    {
        Thread appsThread = new Thread(delegate ()
        {
            Dispatcher.Dispatch(() =>
            {
                refresh.IsEnabled = false;
                loading.IsVisible = true;
                busy.IsRunning = true;
                refresh.IsRefreshing = false;
            });
            RefreshingApps();
        });
        appsThread.Start();
    }

    async void RefreshingApps()
    {
        bool isConnected = await Web.IsConnectedToInternetAsync();
        if (isConnected) Dispatcher.Dispatch(() => internet.IsVisible = false);
        if (!isConnected) Dispatcher.Dispatch(() => internet.IsVisible = true);
        List<ContextClasses.AppContext> apps = Web.GetApps();
        Dispatcher.Dispatch(() => appsLayout.Children.Clear());

        if (apps.Count > 0)
        {
            Dispatcher.Dispatch(() => nothing.IsVisible = false);
            foreach (ContextClasses.AppContext s in apps)
            {
                AppView appView = new AppView();
                appView.App = s;
                appView.AppClicked += Clicked;

                Dispatcher.Dispatch(() => appsLayout.Children.Add(appView));
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
        AppsRefresh();
    }

    private void Clicked(object sender, ContextClasses.AppContext e)
    {
        Navigation.PushAsync(new AppChangelog(e.name));
    }
}