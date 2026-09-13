using Adeptstack_App.ContextClasses;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace Adeptstack_App.Utils
{
    internal class Utilities
    {
        public const string WebsiteUrl = "https://www.adeptstack.net";

        /// <summary>
        /// Link auf den Blogartikel. Die API liefert den Slug mit, falls er fehlt
        /// wird er wie auf der Website aus dem Titel gebildet.
        /// </summary>
        public static string GetNewsUrl(NewsContext news)
        {
            string slug = string.IsNullOrWhiteSpace(news.slug) ? Slugify(news.title) : news.slug;
            return $"{WebsiteUrl}/blog/{slug}";
        }

        /// <summary>
        /// Einzelne Changelogs haben auf der Website keine eigene Seite,
        /// deshalb zeigt der Link auf die Changelog-Übersicht der App.
        /// </summary>
        public static string GetChangelogUrl(string appSlug)
        {
            return string.IsNullOrWhiteSpace(appSlug) ? $"{WebsiteUrl}/changelogs" : $"{WebsiteUrl}/changelogs/{appSlug}";
        }

        // source=app: die Website zeigt Besuchern über geteilte Links ein Popup zum App-Download.
        private const string AppSourceQuery = "?source=app";
        private const string ShareSignature = "Shared via the Adeptstack App";

        public static string GetNewsShareText(NewsContext news)
        {
            return $"Hey! I found this article on Adeptstack and thought you might like it: \"{news.title}\"\n" +
                   $"{GetNewsUrl(news)}{AppSourceQuery}\n\n" +
                   ShareSignature;
        }

        public static string GetChangelogShareText(ChangelogContext changelog, string url)
        {
            return $"Hey! {changelog.title} is out. Take a look at what's new:\n" +
                   $"{url}{AppSourceQuery}\n\n" +
                   ShareSignature;
        }

        /// <summary>
        /// Der Link steckt bewusst im Text statt in ShareTextRequest.Uri: Windows übergibt Text und
        /// Link als getrennte Formate, und viele Ziel-Apps übernehmen nur eins davon.
        /// </summary>
        public static async Task ShareAsync(string title, string text)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = title,
                    Text = text
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        // "PC-Info v3.2.0 (4.1.0)" -> "pc-info-v3-2-0-4-1-0"
        public static string Slugify(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }

            var builder = new StringBuilder();
            foreach (char c in text.ToLowerInvariant().Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return Regex.Replace(builder.ToString(), "[^a-z0-9]+", "-").Trim('-');
        }

        //public static bool IsConnectedToInternet()
        //{
        //    string host = "adeptstack.vercel.app";
        //    bool result = false;
        //    Ping p = new Ping();
        //    try
        //    {
        //        PingReply reply = p.Send(host, 3000);
        //        if (reply.Status == IPStatus.Success)
        //            return true;
        //    }
        //    catch { }
        //    return result;
        //}

        //public static string GetTitle(string url)
        //{
        //    HttpClient client = new HttpClient();
        //    string html = client.GetStringAsync(url).Result;
        //    string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
        //    return title;
        //}
    }
}
