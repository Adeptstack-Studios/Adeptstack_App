using Adeptstack_App.ContextClasses;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Adeptstack_App.Net
{
    internal class Web
    {
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };

        public static async Task<bool> IsConnectedToInternetAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, "https://clients3.google.com/generate_204");
                using (var response = await _httpClient.SendAsync(request))
                {
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Connection test failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// /products?include=unlisted liefert gelistete und ungelistete Produkte.
        /// Solange der Endpoint noch nicht live ist, wird auf /apps/get zurückgegriffen.
        /// </summary>
        public static List<ContextClasses.AppContext> GetApps()
        {
            return GetAppsFrom("https://api.adeptstack.net/api/products?include=unlisted")
                ?? GetAppsFrom("https://api.adeptstack.net/api/apps/get")
                ?? new();
        }

        private static List<ContextClasses.AppContext> GetAppsFrom(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = client.GetStringAsync(url).Result;
                var result = JsonSerializer.Deserialize<List<ContextClasses.AppContext>>(html);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
        }

        public static ContextClasses.AppContext GetAppById(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = client.GetStringAsync($"https://api.adeptstack.net/api/apps/get/{id}").Result;
                var result = JsonSerializer.Deserialize<ContextClasses.AppContext>(html);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return new();
            }
        }

        public static List<ChangelogContext> GetChangelogs(int appId)
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = client.GetStringAsync($"https://api.adeptstack.net/api/changelogs/getBy?appId={appId}").Result;
                var result = JsonSerializer.Deserialize<List<ChangelogContext>>(html);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return new();
            }
        }

        public static List<NewsContext> GetNews()
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = client.GetStringAsync("https://api.adeptstack.net/api/news/get").Result;
                var result = JsonSerializer.Deserialize<List<NewsContext>>(html);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return new();
            }
        }
    }
}
