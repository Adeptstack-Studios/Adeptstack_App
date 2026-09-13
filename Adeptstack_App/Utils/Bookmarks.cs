using Adeptstack_App.ContextClasses;
using System.Diagnostics;
using System.Text.Json;

namespace Adeptstack_App.Utils
{
    /// <summary>
    /// Lesezeichen liegen als JSON-Datei im App-Verzeichnis und nicht in den Preferences:
    /// Windows erlaubt dort nur 8 KB pro Wert, ein Artikel samt Content ist schnell größer.
    /// Gespeichert werden die kompletten Objekte, damit gemerkte Beiträge auch offline lesbar sind.
    /// </summary>
    static class Bookmarks
    {
        private class BookmarkStore
        {
            public List<NewsContext> news { get; set; } = new();
            public List<ChangelogContext> changelogs { get; set; } = new();
        }

        private static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "bookmarks.json");
        private static readonly object Sync = new();
        private static BookmarkStore _store;

        public static event EventHandler Changed;

        public static List<NewsContext> GetNews()
        {
            lock (Sync) return Load().news.ToList();
        }

        public static List<ChangelogContext> GetChangelogs()
        {
            lock (Sync) return Load().changelogs.ToList();
        }

        public static bool IsBookmarked(NewsContext news)
        {
            lock (Sync) return Load().news.Any(n => n.id == news.id);
        }

        public static bool IsBookmarked(ChangelogContext changelog)
        {
            lock (Sync) return Load().changelogs.Any(c => c.id == changelog.id);
        }

        /// <returns>true, wenn der Beitrag danach gemerkt ist.</returns>
        public static bool Toggle(NewsContext news)
        {
            bool bookmarked;
            lock (Sync) bookmarked = Toggle(Load().news, news, n => n.id == news.id);

            Changed?.Invoke(null, EventArgs.Empty);
            return bookmarked;
        }

        /// <returns>true, wenn der Changelog danach gemerkt ist.</returns>
        public static bool Toggle(ChangelogContext changelog)
        {
            bool bookmarked;
            lock (Sync) bookmarked = Toggle(Load().changelogs, changelog, c => c.id == changelog.id);

            Changed?.Invoke(null, EventArgs.Empty);
            return bookmarked;
        }

        private static bool Toggle<T>(List<T> list, T item, Predicate<T> matches)
        {
            bool bookmarked = list.RemoveAll(matches) == 0;

            // Neueste Lesezeichen zuerst
            if (bookmarked)
            {
                list.Insert(0, item);
            }

            Save();
            return bookmarked;
        }

        private static BookmarkStore Load()
        {
            if (_store != null)
            {
                return _store;
            }

            try
            {
                if (File.Exists(FilePath))
                {
                    _store = JsonSerializer.Deserialize<BookmarkStore>(File.ReadAllText(FilePath));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            _store ??= new BookmarkStore();
            _store.news ??= new();
            _store.changelogs ??= new();
            return _store;
        }

        private static void Save()
        {
            try
            {
                File.WriteAllText(FilePath, JsonSerializer.Serialize(_store));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
