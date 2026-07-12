using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Microsoft.Maui.Layouts;
using AppContext = Adeptstack_App.ContextClasses.AppContext;

namespace Adeptstack_App;

public partial class Changelogs : ContentPage
{
    public Changelogs()
    {
        InitializeComponent();
        this.Title = "Adeptstack Changelogs";
        AppsRefresh();
    }

    private async void AppsRefresh()
    {
        refresh.IsEnabled = false;
        loading.IsVisible = true;
        busy.IsRunning = true;
        nothing.IsVisible = false;

        await Task.Run(async () =>
        {
            bool isConnected = await Web.IsConnectedToInternetAsync();
            List<AppContext> apps = new List<AppContext>();

            if (isConnected)
            {
                apps = Web.GetApps();
                apps = apps.OrderBy(a => a.legacy).ToList();
            }

            Dispatcher.Dispatch(() =>
            {
                internet.IsVisible = !isConnected;
                appsLayout.Children.Clear();

                if (apps.Count > 0)
                {
                    nothing.IsVisible = false;

                    foreach (AppContext s in apps)
                    {
                        AppView appView = new AppView { App = s };
                        appView.AppClicked += Clicked;
                        appsLayout.Children.Add(appView);
                    }
                }
                else
                {
                    nothing.IsVisible = true;
                }

                refresh.IsRefreshing = false;
                refresh.IsEnabled = true;
                loading.IsVisible = false;
                busy.IsRunning = false;
            });
        });
    }

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
        AppsRefresh();
    }

    private void Clicked(object sender, AppContext a)
    {
        Navigation.PushAsync(new AppChangelog(a));
    }
}