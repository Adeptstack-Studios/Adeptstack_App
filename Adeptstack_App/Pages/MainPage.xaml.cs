using Adeptstack_App.ContentViews;
using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;

namespace Adeptstack_App;

public partial class MainPage : ContentPage
{
    // Globale Liste, um die Original-Daten im Speicher zu halten
    private List<NewsContext> _allNews = new List<NewsContext>();
    private string _currentCategory = "All";
    private string _searchQuery = "";

    public MainPage()
    {
        InitializeComponent();
        NewsRefresh();
    }

    private async void NewsRefresh()
    {
        refresh.IsEnabled = false;
        loading.IsVisible = true;
        busy.IsRunning = true;
        nothing.IsVisible = false;

        await Task.Run(async () =>
        {
            bool isConnected = await Web.IsConnectedToInternetAsync();

            if (isConnected)
            {
                _allNews = Web.GetNews();
            }

            Dispatcher.Dispatch(() =>
            {
                internet.IsVisible = !isConnected;

                // Kategorien aus den geladenen Daten extrahieren und UI bauen
                BuildCategoryUI();

                // Liste basierend auf aktuellem Filter/Suche anzeigen
                ApplyFiltersAndRender();

                refresh.IsRefreshing = false;
                refresh.IsEnabled = true;
                loading.IsVisible = false;
                busy.IsRunning = false;
            });
        });
    }

    // --- NEU: Logik für Suche und Kategorien ---

    private void SearchIcon_Clicked(object sender, EventArgs e)
    {
        // Toggle Sichtbarkeit
        bool isSearching = !searchBar.IsVisible;
        searchBar.IsVisible = isSearching;
        headerTitle.IsVisible = !isSearching;

        if (isSearching)
        {
            // Suchleiste ist offen -> Zeige das X-Symbol
            searchIconBtn.Source = "close.png";
            searchBar.Focus();
        }
        else
        {
            // Suchleiste ist geschlossen -> Zurück zur Lupe
            searchIconBtn.Source = "search.png";

            // Suche abbrechen und Liste zurücksetzen
            searchBar.Text = string.Empty;
        }
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchQuery = e.NewTextValue?.ToLower() ?? "";
        ApplyFiltersAndRender();
    }

    private void BuildCategoryUI()
    {
        categoryLayout.Children.Clear();

        // Einzigartige Kategorien aus der News-Liste ziehen (plus "All" am Anfang)
        var categories = _allNews.Select(n => n.category).Distinct().ToList();
        categories.Insert(0, "All");

        foreach (var category in categories)
        {
            bool isSelected = category == _currentCategory;

            // Die Kategorie-Pille im Adeptstack-Design
            var btn = new Button
            {
                Text = category.ToUpper(),
                FontFamily = "OpenSansSemibold",
                FontSize = 12,
                CornerRadius = 20,
                HeightRequest = 36,
                Padding = new Thickness(16, 0),
                BackgroundColor = isSelected ? Color.FromArgb("#3b82f6") : Color.FromArgb("#1e293b"),
                TextColor = isSelected ? Colors.White : Color.FromArgb("#94a3b8")
            };

            btn.Clicked += (s, e) =>
            {
                _currentCategory = category;
                BuildCategoryUI(); // Button-Farben updaten
                ApplyFiltersAndRender(); // Liste filtern
            };

            categoryLayout.Children.Add(btn);
        }
    }

    private void ApplyFiltersAndRender()
    {
        // 1. Filtern
        var filteredNews = _allNews.Where(n =>
        {
            bool matchesCategory = _currentCategory == "All" || string.Equals(n.category, _currentCategory, StringComparison.OrdinalIgnoreCase);
            bool matchesSearch = string.IsNullOrWhiteSpace(_searchQuery) ||
                                 (n.title != null && n.title.ToLower().Contains(_searchQuery));

            return matchesCategory && matchesSearch;
        }).ToList();

        // 2. Rendern
        newsLayout.Children.Clear();

        if (filteredNews.Count > 0)
        {
            nothing.IsVisible = false;
            foreach (NewsContext newsItem in filteredNews)
            {
                NewsView newsView = new NewsView { News = newsItem };
                newsView.NewsClicked += News_Clicked;
                newsLayout.Children.Add(newsView);
            }
        }
        else
        {
            // Text anpassen, falls Suche/Filter leer ist, aber Internet da ist
            emptyStateLabel.Text = _allNews.Count > 0 ? "No results found" : "No Updates Available";
            nothing.IsVisible = true;
        }
    }

    // --- Vorhandene Events ---

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
        NewsRefresh();
    }

    private void News_Clicked(object sender, NewsContext e)
    {
        Navigation.PushAsync(new DisplayContent(e));
    }
}