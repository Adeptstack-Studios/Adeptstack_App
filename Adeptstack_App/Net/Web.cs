using Adeptstack_App.ContextClasses;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Adeptstack_App.Net
{
    internal class Web
    {
        public static bool IsConnectedToInternet()
        {
            string host = "adeptstack.net";
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { }
            return result;
        }

        public static List<string> GetApps()
        {
            HttpClient client = new HttpClient();
            string html = client.GetStringAsync("https://api.adeptstack.net/api/changelogs/get").Result;
            var result = JsonSerializer.Deserialize<List<ChangelogContext>>(html);

            List<string> apps = result
                .Select(c => c.app)
                .Distinct()
                .ToList();

            return apps;
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
                string error = e.ToString();
                Console.WriteLine(error);
            }
            return new();
        }

        public static List<NewsContext> GetNews()
        {
            HttpClient client = new HttpClient();
            string html = client.GetStringAsync("https://api.adeptstack.net/api/news/get").Result;
            var result = JsonSerializer.Deserialize<List<NewsContext>>(html);
            return result;
        }
    }
}
