using Adeptstack_App.ContextClasses;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Adeptstack_App.Net
{
    internal class Web
    {
        private const string ApiBaseUrl = "https://api.adeptstack.net/api";

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        private static readonly HttpClient _apiClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        // Für die Gegenprobe an der eigenen API, wenn der schnelle Test fehlschlägt
        private static readonly TimeSpan ApiProbeTimeout = TimeSpan.FromSeconds(5);

        public static async Task<bool> IsConnectedToInternetAsync()
        {
            // Flugmodus oder gar kein Netz: nicht erst auf Timeouts warten
            if (Connectivity.Current.NetworkAccess == NetworkAccess.None)
            {
                return false;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, "https://clients3.google.com/generate_204");
                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Connection test failed: {e.Message}");
            }

            return await IsApiReachableAsync();
        }

        /// <summary>
        /// Google kann gesperrt oder langsam sein, obwohl die eigene API erreichbar ist.
        /// Jede Antwort zählt, auch ein Fehlerstatus: dann steht die Verbindung.
        /// </summary>
        private static async Task<bool> IsApiReachableAsync()
        {
            try
            {
                using var timeout = new CancellationTokenSource(ApiProbeTimeout);
                var request = new HttpRequestMessage(HttpMethod.Head, $"{ApiBaseUrl}/news/categories");
                using var response = await _apiClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"API probe failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// include=unlisted liefert gelistete und ungelistete Apps.
        /// </summary>
        public static async Task<List<ContextClasses.AppContext>> GetAppsAsync()
        {
            try
            {
                string json = await _apiClient.GetStringAsync($"{ApiBaseUrl}/apps/get?include=unlisted").ConfigureAwait(false);
                return JsonSerializer.Deserialize<List<ContextClasses.AppContext>>(json) ?? new();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return new();
            }
        }

        /// <returns>null, wenn die App nicht geladen werden konnte.</returns>
        public static async Task<ContextClasses.AppContext> GetAppByIdAsync(int id)
        {
            try
            {
                string json = await _apiClient.GetStringAsync($"{ApiBaseUrl}/apps/get/{id}").ConfigureAwait(false);
                return JsonSerializer.Deserialize<ContextClasses.AppContext>(json);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
        }

        // --- Gefilterte Listen mit Pagination ---

        /// <summary>
        /// Eine Seite einer Liste. Total kommt aus dem Header X-Total-Count und zählt alle Treffer, nicht nur diese Seite.
        /// Success ist false bei Netzwerkfehlern und Fehlerstatus (400 bei ungültigen Parametern, 403/429 beim Rate-Limit).
        /// </summary>
        public record PagedResult<T>(List<T> Items, int Total, bool Success);

        /// <param name="category">null = alle Kategorien.</param>
        /// <param name="sort">z. B. "publishedAt,desc" oder "readingTime,asc".</param>
        public static Task<PagedResult<NewsContext>> GetNewsPageAsync(string category, string query, string sort, int page, int size)
        {
            return GetPageAsync<NewsContext>("/news/get", new()
            {
                ["include"] = "unlisted",
                ["category"] = category,
                ["q"] = query,
                ["sort"] = sort,
                ["page"] = page.ToString(),
                ["size"] = size.ToString()
            });
        }

        public static async Task<List<FilterOption>> GetNewsCategoriesAsync()
        {
            return (await GetPageAsync<FilterOption>("/news/categories", new() { ["include"] = "unlisted" })).Items;
        }

        /// <param name="app">App-ID oder Slug, null = alle Apps.</param>
        /// <param name="channel">null = alle Channels.</param>
        /// <param name="sort">z. B. "publishedAt,desc" oder "version,desc".</param>
        public static Task<PagedResult<ChangelogContext>> GetChangelogsPageAsync(string app, string channel, string sort, int page, int size)
        {
            return GetPageAsync<ChangelogContext>("/changelogs/get", new()
            {
                ["include"] = "unlisted",
                ["app"] = app,
                ["channel"] = channel,
                ["sort"] = sort,
                ["page"] = page.ToString(),
                ["size"] = size.ToString()
            });
        }

        /// <param name="app">App-ID oder Slug, null = Channels aller Apps.</param>
        public static async Task<List<FilterOption>> GetChangelogChannelsAsync(string app)
        {
            return (await GetPageAsync<FilterOption>("/changelogs/channels", new() { ["include"] = "unlisted", ["app"] = app })).Items;
        }

        private static async Task<PagedResult<T>> GetPageAsync<T>(string path, Dictionary<string, string> query)
        {
            string queryString = string.Join("&", query
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value.Trim())}"));

            try
            {
                // ConfigureAwait(false): Changelogs enthalten den kompletten Markdown-Content,
                // das Parsen soll nicht auf dem UI-Thread laufen.
                using var response = await _apiClient.GetAsync($"{ApiBaseUrl}{path}?{queryString}").ConfigureAwait(false);
                string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    // Bei 400 steht der Grund im Feld "message"
                    Debug.WriteLine($"GET {path}?{queryString} -> {(int)response.StatusCode}: {body}");
                    return new(new(), 0, false);
                }

                var items = JsonSerializer.Deserialize<List<T>>(body) ?? new();
                int total = response.Headers.TryGetValues("X-Total-Count", out var values) && int.TryParse(values.FirstOrDefault(), out int count)
                    ? count
                    : items.Count;

                return new(items, total, true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return new(new(), 0, false);
            }
        }
    }
}
