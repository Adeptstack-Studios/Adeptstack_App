using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
using Adeptstack_App.Pages;
using Adeptstack_App.Utils;

namespace Adeptstack_App;

public partial class MainPage : ContentPage
{
    // Anzeigename -> sort-Parameter der API
    private static readonly (string Label, string Sort)[] SortOptions =
    {
        ("Newest", "publishedAt,desc"),
        ("Oldest", "publishedAt,asc"),
        ("A–Z", "title,asc"),
        ("Shortest first", "readingTime,asc"),
        ("Longest first", "readingTime,desc")
    };

    private readonly PagedLoader<NewsContext> _loader;
    private List<FilterOption> _categories = new List<FilterOption>();
    private string _currentCategory; // null = alle
    private string _searchQuery = "";
    private int _sortIndex = 0;
    private CancellationTokenSource _searchDebounce;

    public MainPage()
    {
        InitializeComponent();

        _loader = new PagedLoader<NewsContext>(
            (page, size) => Web.GetNewsPageAsync(_currentCategory, _searchQuery, SortOptions[_sortIndex].Sort, page, size),
            n => n.id);

        NewsRefresh();
    }

    private async void NewsRefresh()
    {
        refresh.IsEnabled = false;
        loading.IsVisible = true;
        busy.IsRunning = true;
        nothing.IsVisible = false;

        bool isConnected = await Web.IsConnectedToInternetAsync();
        internet.IsVisible = !isConnected;

        if (isConnected)
        {
            _categories = await Web.GetNewsCategoriesAsync();

            if (!_categories.Any(c => string.Equals(c.name, _currentCategory, StringComparison.OrdinalIgnoreCase)))
            {
                _currentCategory = null;
            }
        }

        BuildCategoryUI();
        await ReloadAsync();

        refresh.IsRefreshing = false;
        refresh.IsEnabled = true;
        loading.IsVisible = false;
        busy.IsRunning = false;
    }

    // --- Suche, Kategorien und Sortierung ---

    private void SearchIcon_Clicked(object sender, EventArgs e)
    {
        bool isSearching = !searchBar.IsVisible;
        searchBar.IsVisible = isSearching;
        headerTitle.IsVisible = !isSearching;

        if (isSearching)
        {
            searchIconBtn.Source = "close.png";
            searchBar.Focus();
        }
        else
        {
            searchIconBtn.Source = "search.png";
            searchBar.Text = string.Empty;
        }
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = e.NewTextValue?.Trim() ?? "";

        // Erst 300 ms nach dem letzten Tastendruck suchen, nicht bei jedem Zeichen
        _searchDebounce?.Cancel();
        var debounce = _searchDebounce = new CancellationTokenSource();

        try
        {
            await Task.Delay(300, debounce.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (query == _searchQuery)
        {
            return;
        }

        _searchQuery = query;
        await ReloadAsync();
    }

    private void BuildCategoryUI()
    {
        var chips = new List<FilterChips.Chip> { FilterChips.All(_categories) };
        chips.AddRange(_categories.Select(FilterChips.FromOption));

        FilterChips.Build(categoryLayout, chips, _currentCategory, async category =>
        {
            _currentCategory = category;
            BuildCategoryUI();
            await ReloadAsync();
        });
    }

    private async void Sort_Clicked(object sender, EventArgs e)
    {
        int index = await SelectionSheet.PickAsync(this, "Sort by", SortOptions.Select(o => o.Label).ToList(), _sortIndex);

        if (index < 0 || index == _sortIndex)
        {
            return;
        }

        _sortIndex = index;
        sortButton.Text = $"{SortOptions[index].Label.ToUpper()} ▾";
        await ReloadAsync();
    }

    // --- Laden mit Pagination ---

    /// <summary>
    /// Leert die Liste und beginnt wieder bei Seite 0.
    /// </summary>
    private async Task ReloadAsync()
    {
        _loader.Reset();

        newsLayout.Children.Clear();
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

        foreach (NewsContext newsItem in page.NewItems)
        {
            NewsView newsView = new NewsView { News = newsItem };
            newsView.NewsClicked += News_Clicked;
            newsLayout.Children.Add(newsView);
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
        bool isFiltered = _currentCategory != null || !string.IsNullOrEmpty(_searchQuery);

        resultCountLabel.Text = total == 1 ? "1 article" : $"{total} articles";

        // Ohne Filter heißt eine leere Liste: keine Verbindung oder keine Beiträge. Dann das große Overlay.
        nothing.IsVisible = total == 0 && (!success || !isFiltered);
        noResultsLabel.IsVisible = total == 0 && success && isFiltered;
    }

    private async void LoadMoreIfNeeded()
    {
        if (_loader.HasMorePages && PagedLoader<NewsContext>.IsNearBottom(scroll))
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
        NewsRefresh();
    }

    private void News_Clicked(object sender, NewsContext e)
    {
        Navigation.PushAsync(new DisplayContent(e));
    }
}
