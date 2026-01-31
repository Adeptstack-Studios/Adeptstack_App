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

    void RefreshingApps()
    {
        List<string> apps = Web.GetApps();
        Dispatcher.Dispatch(() => appsLayout.Children.Clear());

        foreach (string s in apps)
        {
            var label = new Label
            {
                Text = s,
                FontSize = 18,
                FontFamily = "OpenSans",
                TextColor = Color.FromArgb("#dddddd"),
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            };

            var button = new Border
            {
                HeightRequest = 60,
                Margin = new Thickness(0, 0, 0, 10),
                BackgroundColor = Color.FromArgb("#171717"),
                Padding = new Thickness(30, 0),

                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(12)
                },

                Content = label
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += Clicked;

            button.GestureRecognizers.Add(tapGesture);

            Dispatcher.Dispatch(() => appsLayout.Children.Add(button));
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

    private void Clicked(object sender, EventArgs e)
    {
        Border clickedBorder = sender as Border;
        Label textLabel = clickedBorder.Content as Label;

        if (textLabel != null)
        {
            Navigation.PushAsync(new AppChangelog(textLabel.Text));
        }
    }
}