using Adeptstack_App.Net;

namespace Adeptstack_App.Utils
{
    /// <summary>
    /// Zustand fürs Endlos-Scrollen über die paginierten /get-Endpunkte.
    /// Die Seite kümmert sich nur noch um die Anzeige.
    /// </summary>
    class PagedLoader<T>
    {
        public const int DefaultPageSize = 10;

        // Ab diesem Abstand zum Ende wird die nächste Seite geladen
        private const double LoadMoreThreshold = 800;

        public record Page(List<T> NewItems, bool IsFirstPage, bool Success);

        private readonly Func<int, int, Task<Web.PagedResult<T>>> _fetch;
        private readonly Func<T, int> _getId;
        private readonly int _pageSize;

        // Neue Einträge zwischen zwei Seitenaufrufen verschieben die Seiten um eins, deshalb per id deduplizieren.
        private readonly HashSet<int> _loadedIds = new HashSet<int>();
        private int _nextPage;
        private int _total;

        // Jeder Reset erhöht die Generation. Antworten älterer Anfragen werden verworfen.
        private int _generation;

        /// <param name="fetch">Lädt (page, size). Filter und Sortierung liest die Seite beim Aufruf selbst aus.</param>
        public PagedLoader(Func<int, int, Task<Web.PagedResult<T>>> fetch, Func<T, int> getId, int pageSize = DefaultPageSize)
        {
            _fetch = fetch;
            _getId = getId;
            _pageSize = pageSize;
        }

        /// <summary>
        /// Anzahl aller Treffer laut X-Total-Count, nicht nur der geladenen.
        /// </summary>
        public int Total => _total;

        public bool IsLoading { get; private set; }

        public bool HasMorePages => _loadedIds.Count < _total;

        /// <summary>
        /// Nach jedem Filter- oder Sortierwechsel: wieder bei Seite 0 beginnen.
        /// </summary>
        public void Reset()
        {
            _generation++;
            _loadedIds.Clear();
            _nextPage = 0;
            _total = 0;
            IsLoading = false;
        }

        /// <returns>null, wenn schon geladen wird oder die Antwort durch einen Reset veraltet ist.</returns>
        public async Task<Page> LoadNextPageAsync()
        {
            if (IsLoading)
            {
                return null;
            }

            int generation = _generation;
            bool firstPage = _nextPage == 0;
            IsLoading = true;

            var result = await _fetch(_nextPage, _pageSize);

            if (generation != _generation)
            {
                return null;
            }

            IsLoading = false;

            if (!result.Success || result.Items.Count == 0)
            {
                // Nicht endlos weiterversuchen, Pull-to-Refresh lädt neu
                _total = _loadedIds.Count;
                return new Page(new List<T>(), firstPage, result.Success);
            }

            _total = result.Total;
            _nextPage++;

            return new Page(result.Items.Where(item => _loadedIds.Add(_getId(item))).ToList(), firstPage, true);
        }

        public static bool IsNearBottom(ScrollView scroll)
        {
            // Vor dem ersten Layout ist alles 0 und würde sonst als "unten angekommen" zählen
            if (scroll.Height <= 0 || scroll.ContentSize.Height <= 0)
            {
                return false;
            }

            return scroll.ScrollY + scroll.Height >= scroll.ContentSize.Height - LoadMoreThreshold;
        }
    }
}
