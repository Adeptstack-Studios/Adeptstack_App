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

        public static List<ContextClasses.AppContext> GetApps()
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = client.GetStringAsync("https://api.adeptstack.net/api/apps/get").Result;
                var result = JsonSerializer.Deserialize<List<ContextClasses.AppContext>>(html);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return new();
            }
        }

        public static List<ChangelogContext> GetChangelogs(string app)
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = client.GetStringAsync($"https://api.adeptstack.net/api/changelogs/getBy?app={app}").Result;
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
