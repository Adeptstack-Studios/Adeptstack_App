using Adeptstack_App.ContextClasses;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Adeptstack_App.Net
{
    internal class Web
    {
        public static bool IsConnectedToInternet()
        {
            string host = "adeptstack.vercel.app";
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

        public static void GetMetaData()
        {
            HttpClient client = new HttpClient();
            string html = client.GetStringAsync("https://adeptstack.vercel.app/meta.json").Result;

        }

        public static List<ChangelogContext> GetChangelogs(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = client.GetStringAsync(url).Result;
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

        public static string GetChangelogContent(string url)
        {
            HttpClient client = new HttpClient();
            string md = client.GetStringAsync(url).Result;
            return md;
        }

        public static List<NewsContext> GetNews()
        {
            HttpClient client = new HttpClient();
            string html = client.GetStringAsync("https://adeptstack.vercel.app/News.json").Result;
            var result = JsonSerializer.Deserialize<List<NewsContext>>(html);
            return result;
        }
    }
}
