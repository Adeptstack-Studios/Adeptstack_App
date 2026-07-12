using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using AppContext = Adeptstack_App.ContextClasses.AppContext;

namespace Adeptstack_App;

public partial class AppChangelog : ContentPage
{
    private AppContext app;

    public AppChangelog(AppContext app)
    {
        InitializeComponent();
        this.app = app;
        this.Title = $"{app.name} Updates"; // Wirkt etwas moderner als "Changelogs"

        legacy.IsVisible = app.legacy;
        ChangelogsRefreshAsync();
    }

    private async void ChangelogsRefreshAsync()
    {
        // UI für Ladevorgang sperren
        refresh.IsEnabled = false;
        loading.IsVisible = true;
        busy.IsRunning = true;
        nothing.IsVisible = false;

        // Ab in den Hintergrund-Thread
        await Task.Run(async () =>
        {
            bool isConnected = await Web.IsConnectedToInternetAsync();
            List<ChangelogContext> changelogs = new List<ChangelogContext>();

            if (isConnected)
            {
                changelogs = Web.GetChangelogs(this.app.id);

                // Sortieren: Neuestes Datum zuerst!
                if (changelogs != null && changelogs.Count > 0)
                {
                    changelogs = changelogs.OrderByDescending(c => c.publishedAt).ToList();
                }
            }

            // Zurück auf den Main-Thread für das UI-Update
            Dispatcher.Dispatch(() =>
            {
                internet.IsVisible = !isConnected;
                changelogsLayout.Children.Clear();

                if (changelogs != null && changelogs.Count > 0)
                {
                    nothing.IsVisible = false;
                    foreach (ChangelogContext changelogItem in changelogs)
                    {
                        ChangelogView changelogView = new ChangelogView
                        {
                            Changelog = changelogItem,
                            Margin = new Thickness(8, 8, 8, 16) // Sorgt für saubere Abstände zwischen den Karten
                        };
                        changelogView.ChangelogClicked += Changelog_Clicked;

                        changelogsLayout.Children.Add(changelogView);
                    }
                }
                else
                {
                    nothing.IsVisible = true;
                }

                // Loading abschließen
                refresh.IsRefreshing = false;
                refresh.IsEnabled = true;
                loading.IsVisible = false;
                busy.IsRunning = false;
            });
        });
    }

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
        ChangelogsRefreshAsync();
    }

    private void Changelog_Clicked(object sender, ChangelogContext e)
    {
        // Da die DisplayContent-Seite jetzt die ID direkt als API-Call umwandelt (haben wir vorher repariert!), 
        // klappt das hier nahtlos.
        Navigation.PushAsync(new DisplayContent(e));
    }
}