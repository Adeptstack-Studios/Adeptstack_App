using System.Globalization;

namespace Adeptstack_App.Utils
{
    /// <summary>
    /// Lokale Einstellungen der App, gespeichert in den Preferences.
    /// </summary>
    static class AppSettings
    {
        // --- Lesen ---

        /// <summary>
        /// Schriftgröße in Artikeln und Changelogs, in Prozent der normalen Größe.
        /// </summary>
        public static readonly (string Label, int Percent)[] TextSizes =
        {
            ("Small", 90),
            ("Default", 100),
            ("Large", 115),
            ("Extra large", 130)
        };

        public static int TextSizeIndex
        {
            get => Math.Clamp(Preferences.Default.Get(nameof(TextSizeIndex), 1), 0, TextSizes.Length - 1);
            set => Preferences.Default.Set(nameof(TextSizeIndex), value);
        }

        public static int TextSizePercent => TextSizes[TextSizeIndex].Percent;

        /// <summary>
        /// true: Links öffnen im In-App-Browser (Custom Tabs / SFSafariViewController), sonst im Standardbrowser.
        /// </summary>
        public static bool OpenLinksInApp
        {
            get => Preferences.Default.Get(nameof(OpenLinksInApp), true);
            set => Preferences.Default.Set(nameof(OpenLinksInApp), value);
        }

        public static Task OpenLinkAsync(string url)
        {
            return Browser.Default.OpenAsync(url, OpenLinksInApp ? BrowserLaunchMode.SystemPreferred : BrowserLaunchMode.External);
        }

        // --- Filter und Sortierung ---

        /// <summary>
        /// Sortierung und Filter beim nächsten Öffnen wiederherstellen.
        /// </summary>
        public static bool RememberFilters
        {
            get => Preferences.Default.Get(nameof(RememberFilters), true);
            set
            {
                Preferences.Default.Set(nameof(RememberFilters), value);

                if (!value)
                {
                    Preferences.Default.Clear(FilterSharedName);
                }
            }
        }

        // Eigener Bereich, damit sich die gemerkten Filter auf einen Schlag löschen lassen
        private const string FilterSharedName = "filters";

        /// <returns>Den gemerkten Wert oder null, wenn nichts gemerkt ist oder das Merken aus ist.</returns>
        public static string GetFilter(string key)
        {
            return RememberFilters ? Preferences.Default.Get<string>(key, null, FilterSharedName) : null;
        }

        /// <param name="value">null entfernt den Wert, z. B. beim "All"-Chip.</param>
        public static void SetFilter(string key, string value)
        {
            if (!RememberFilters)
            {
                return;
            }

            if (value == null)
            {
                Preferences.Default.Remove(key, FilterSharedName);
            }
            else
            {
                Preferences.Default.Set(key, value, FilterSharedName);
            }
        }

        /// <summary>
        /// Index der gemerkten Sortierung. Gespeichert wird der API-Wert, damit
        /// neue oder umsortierte Optionen keinen falschen Eintrag treffen.
        /// </summary>
        public static int GetSortIndex(string key, (string Label, string Sort)[] options)
        {
            string sort = GetFilter(key);
            return Math.Max(0, Array.FindIndex(options, o => o.Sort == sort));
        }

        // --- Datensparmodus ---

        /// <summary>
        /// Bilder nur über WLAN oder Ethernet laden.
        /// </summary>
        public static bool DataSaver
        {
            get => Preferences.Default.Get(nameof(DataSaver), false);
            set => Preferences.Default.Set(nameof(DataSaver), value);
        }

        public static bool ShouldLoadImages
        {
            get
            {
                if (!DataSaver)
                {
                    return true;
                }

                var profiles = Connectivity.Current.ConnectionProfiles;
                return profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Ethernet);
            }
        }
    }

    /// <summary>
    /// Liefert im Datensparmodus ohne WLAN keine Bild-URL, damit gar nicht erst geladen wird.
    /// </summary>
    class DataSaverImageConverter : IValueConverter
    {
        public static readonly DataSaverImageConverter Instance = new DataSaverImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!AppSettings.ShouldLoadImages || value is not string url || string.IsNullOrEmpty(url))
            {
                return null;
            }

            return (ImageSource)url;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
