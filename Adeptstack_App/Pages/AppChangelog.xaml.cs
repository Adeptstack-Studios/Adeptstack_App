using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Pages;
using Adeptstack_App.Utils;
using AppContext = Adeptstack_App.ContextClasses.AppContext;

namespace Adeptstack_App;

/// <summary>
/// Changelogs einer App oder, als "All Updates", aller Apps mit App-Auswahl.
/// </summary>
public partial class AppChangelog : ContentPage
{
    // Anzeigename -> sort-Parameter der API
    private static readonly (string Label, string Sort)[] SortOptions =
    {
        ("Newest", "publishedAt,desc"),
        ("Oldest", "publishedAt,asc"),
        ("Highest version", "version,desc"),
        ("Lowest version", "version,asc"),
        ("A–Z", "title,asc")
    };

    // Nur sinnvoll, wenn mehrere Apps in der Liste stehen
    private static readonly (string Label, string Sort) AppSortOption = ("App A–Z", "app,asc");

    private readonly List<AppContext> _apps;
    private readonly bool _isAllApps;
    private readonly (string Label, string Sort)[] _sortOptions;
    private readonly PagedLoader<ChangelogContext> _loader;

    private AppContext _selectedApp; // null = alle
    private string _channel; // null = alle
    private List<FilterOption> _channels = new List<FilterOption>();
    private int _sortIndex = 0;

    public AppChangelog(AppContext app) : this(new List<AppContext> { app }, app)
    {
        this.Title = $"{app.name} Updates";
        legacy.IsVisible = app.legacy;
    }

    /// <param name="apps">Für die App-Chips und damit DisplayContent die App nicht erneut laden muss.</param>
    public AppChangelog(List<AppContext> apps) : this(apps, null)
    {
        this.Title = "All Updates";
    }

    private AppChangelog(List<AppContext> apps, AppContext app)
    {
        InitializeComponent();

        _apps = apps;
        _selectedApp = app;
        _isAllApps = app == null;
        _sortOptions = _isAllApps ? SortOptions.Append(AppSortOption).ToArray() : SortOptions;

        _loader = new PagedLoader<ChangelogContext>(
            (page, size) => Web.GetChangelogsPageAsync(_selectedApp?.id.ToString(), _channel, _sortOptions[_sortIndex].Sort, page, size),
            c => c.id);

        appChips.IsVisible = _isAllApps;
        BuildAppChips();
        ChangelogsRefresh();
    }

    private async void ChangelogsRefresh()
    {
        refresh.IsEnabled = false;
        loading.IsVisible = true;
        busy.IsRunning = true;
        nothing.IsVisible = false;

        bool isConnected = await Web.IsConnectedToInternetAsync();
        internet.IsVisible = !isConnected;

        if (isConnected)
        {
            await LoadChannelsAsync();
        }

        await ReloadAsync();

        refresh.IsRefreshing = false;
        refresh.IsEnabled = true;
        loading.IsVisible = false;
        busy.IsRunning = false;
    }

    // --- App, Channel und Sortierung ---

    private void BuildAppChips()
    {
        if (!_isAllApps)
        {
            return;
        }

        var chips = new List<FilterChips.Chip> { new FilterChips.Chip(null, "ALL") };
        chips.AddRange(_apps.Select(a => new FilterChips.Chip(a.id.ToString(), a.name.ToUpper())));

        FilterChips.Build(appLayout, chips, _selectedApp?.id.ToString(), async appId =>
        {
            _selectedApp = _apps.FirstOrDefault(a => a.id.ToString() == appId);

            // Channels gehören zur App, also zurücksetzen
            _channel = null;

            BuildAppChips();
            await LoadChannelsAsync();
            await ReloadAsync();
        });
    }

    private async Task LoadChannelsAsync()
    {
        AppContext app = _selectedApp;
        var channels = await Web.GetChangelogChannelsAsync(app?.id.ToString());

        // Inzwischen eine andere App gewählt
        if (app != _selectedApp)
        {
            return;
        }

        _channels = channels;

        if (!_channels.Any(c => string.Equals(c.name, _channel, StringComparison.OrdinalIgnoreCase)))
        {
            _channel = null;
        }

        BuildChannelChips();
    }

    private void BuildChannelChips()
    {
        // Viele Apps haben nur einen Channel, dann gibt es nichts zu filtern
        channelChips.IsVisible = _channels.Count > 1;

        var chips = new List<FilterChips.Chip> { FilterChips.All(_channels) };
        chips.AddRange(_channels.Select(FilterChips.FromOption));

        FilterChips.Build(channelLayout, chips, _channel, async channel =>
        {
            _channel = channel;
            BuildChannelChips();
            await ReloadAsync();
        });
    }

    private async void Sort_Clicked(object sender, EventArgs e)
    {
        int index = await SelectionSheet.PickAsync(this, "Sort by", _sortOptions.Select(o => o.Label).ToList(), _sortIndex);

        if (index < 0 || index == _sortIndex)
        {
            return;
        }

        _sortIndex = index;
        sortButton.Text = $"{_sortOptions[index].Label.ToUpper()} ▾";
        await ReloadAsync();
    }

    // --- Laden mit Pagination ---

    /// <summary>
    /// Leert die Liste und beginnt wieder bei Seite 0.
    /// </summary>
    private async Task ReloadAsync()
    {
        _loader.Reset();

        changelogsLayout.Children.Clear();
        noResultsLabel.IsVisible = false;
        resultCountLabel.Text = "";

        // Nicht awaiten: ohne Handler (erster Aufruf aus dem Konstruktor) wird der Task nie fertig
        if (scroll.ScrollY > 0)
        {
            _ = scroll.ScrollToAsync(0, 0, false);
        }

        await LoadNextPageAsync();
    }

    private async Task LoadNextPageAsync()
    {
        if (_loader.IsLoading)
        {
            return;
        }

        // Beim ersten Laden zeigt das Overlay schon den Fortschritt
        pageLoading.IsVisible = pageLoading.IsRunning = !loading.IsVisible;

        var page = await _loader.LoadNextPageAsync();

        // Veraltet: die neuere Anfrage kümmert sich um die Anzeige
        if (page == null)
        {
            return;
        }

        pageLoading.IsVisible = pageLoading.IsRunning = false;

        foreach (ChangelogContext changelogItem in page.NewItems)
        {
            ChangelogView changelogView = new ChangelogView
            {
                Changelog = changelogItem,
                Margin = new Thickness(8, 8, 8, 16) // Sorgt für saubere Abstände zwischen den Karten
            };
            changelogView.ChangelogClicked += Changelog_Clicked;

            changelogsLayout.Children.Add(changelogView);
        }

        if (page.IsFirstPage)
        {
            UpdateEmptyState(page.Success);
        }

        // Nur Duplikate geladen: die Höhe ändert sich nicht, also selbst weitermachen
        if (page.NewItems.Count == 0)
        {
            LoadMoreIfNeeded();
        }
    }

    private void UpdateEmptyState(bool success)
    {
        int total = _loader.Total;
        bool isFiltered = _channel != null || (_isAllApps && _selectedApp != null);

        resultCountLabel.Text = total == 1 ? "1 update" : $"{total} updates";

        // Ohne Filter heißt eine leere Liste: keine Verbindung oder keine Updates. Dann das große Overlay.
        nothing.IsVisible = total == 0 && (!success || !isFiltered);
        noResultsLabel.IsVisible = total == 0 && success && isFiltered;
    }

    private async void LoadMoreIfNeeded()
    {
        if (_loader.HasMorePages && PagedLoader<ChangelogContext>.IsNearBottom(scroll))
        {
            await LoadNextPageAsync();
        }
    }

    private void Scroll_Scrolled(object sender, ScrolledEventArgs e)
    {
        LoadMoreIfNeeded();
    }

    // Füllen 10 Karten den Bildschirm nicht (Desktop), gibt es nichts zu scrollen. Dann direkt nachladen.
    private void Content_SizeChanged(object sender, EventArgs e)
    {
        LoadMoreIfNeeded();
    }

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
        ChangelogsRefresh();
    }

    private void Changelog_Clicked(object sender, ChangelogContext e)
    {
        // App mitgeben: DisplayContent braucht den Namen dann nicht erneut von der API.
        Navigation.PushAsync(new DisplayContent(e, _apps.FirstOrDefault(a => a.id == e.appId)));
    }
}
