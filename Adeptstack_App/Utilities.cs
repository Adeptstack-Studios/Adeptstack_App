using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Adeptstack_App
{
    internal class Utilities
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

        public static string GetTitle(string url)
        {
            HttpClient client = new HttpClient();
            string html = client.GetStringAsync(url).Result;
            string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            return title;
        }
    }
}
