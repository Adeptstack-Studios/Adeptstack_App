namespace Adeptstack_App.Utils
{
    /// <summary>
    /// Farben für den hellen und dunklen Modus. Die Seiten holen sie per DynamicResource,
    /// damit ein Wechsel sofort überall greift.
    /// </summary>
    static class AppTheming
    {
        public static readonly (string Label, AppTheme Theme)[] Options =
        {
            ("System", AppTheme.Unspecified),
            ("Light", AppTheme.Light),
            ("Dark", AppTheme.Dark)
        };

        private static readonly Dictionary<string, (string Dark, string Light)> Palette = new()
        {
            // Flächen
            ["PageBackground"] = ("#020617", "#eef4fd"),
            ["Surface"] = ("#0f172a", "#ffffff"),          // Karten, Kopfzeile, Tab-Leiste, Sheets
            ["SurfaceAlt"] = ("#1e293b", "#dbe8fb"),       // Chips, Buttons, Code
            ["Border"] = ("#1e293b", "#d4e2f7"),           // Kartenrand, Trennlinien
            ["BorderStrong"] = ("#334155", "#b9cdea"),
            ["Overlay"] = ("#cc020617", "#cceef4fd"),      // Ladeanzeige über dem Inhalt
            ["Backdrop"] = ("#99020617", "#660f172a"),     // abgedunkelter Hintergrund hinter Sheets
            ["TableStripe"] = ("#0b1220", "#f5f9ff"),

            // Text
            ["TextPrimary"] = ("#f8fafc", "#0f172a"),
            ["TextBody"] = ("#cbd5e1", "#334155"),
            ["TextSecondary"] = ("#94a3b8", "#475569"),
            ["TextMuted"] = ("#64748b", "#64748b"),
            ["TextFaint"] = ("#475569", "#94a3b8"),

            // Akzente. Hell ein dunkleres Blau, #3b82f6 hat auf Weiß zu wenig Kontrast für kleine Schrift.
            ["Accent"] = ("#3b82f6", "#2563eb"),
            ["OnAccent"] = ("#ffffff", "#ffffff"),
            ["BadgeBackground"] = ("#172554", "#dbeafe"),
            ["BadgeBorder"] = ("#1e3a8a", "#bfdbfe"),
            ["BadgeText"] = ("#bfdbfe", "#1e40af"),
            ["Danger"] = ("#f87171", "#dc2626"),
            ["Warning"] = ("#f59e0b", "#b45309"),
            ["WarningText"] = ("#d97706", "#92400e"),
            ["WarningBackground"] = ("#1e1a18", "#fffbeb"),
            ["WarningBorder"] = ("#5c4424", "#fcd34d")
        };

        public static AppTheme Setting
        {
            get => Options.FirstOrDefault(o => o.Theme.ToString() == Preferences.Default.Get(nameof(AppTheming), ""), Options[0]).Theme;
            set
            {
                Preferences.Default.Set(nameof(AppTheming), value.ToString());
                Application.Current.UserAppTheme = value;
                Apply();
            }
        }

        public static bool IsDark => Application.Current?.RequestedTheme != AppTheme.Light;

        /// <summary>
        /// Gespeicherte Einstellung anwenden und dem System-Theme folgen. Einmal beim Start aufrufen.
        /// </summary>
        public static void Initialize(Application app)
        {
            app.UserAppTheme = Setting;
            app.RequestedThemeChanged += (s, e) => Apply();
            Apply();
        }

        public static Color Get(string key)
        {
            return Application.Current.Resources.TryGetValue(key, out var value) ? (Color)value : Colors.Transparent;
        }

        /// <summary>
        /// Symbol passend zum Theme. Die Symbole sind hellgrau, für den hellen Modus gibt es "name_dark".
        /// </summary>
        public static void SetIcon(Image image, string name)
        {
            image.SetAppTheme<ImageSource>(Image.SourceProperty, $"{name}_dark.png", $"{name}.png");
        }

        public static void SetIcon(MenuItem item, string name)
        {
            item.SetAppTheme<ImageSource>(MenuItem.IconImageSourceProperty, $"{name}_dark.png", $"{name}.png");
        }

        /// <summary>
        /// Für CSS im WebView, z. B. "rgba(15, 23, 42, 1)".
        /// </summary>
        public static string Css(string key)
        {
            Color color = Get(key);
            return $"rgba({(int)(color.Red * 255)}, {(int)(color.Green * 255)}, {(int)(color.Blue * 255)}, {color.Alpha.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
        }

        private static void Apply()
        {
            bool dark = IsDark;

            foreach (var (key, colors) in Palette)
            {
                Application.Current.Resources[key] = Color.FromArgb(dark ? colors.Dark : colors.Light);
            }

#if ANDROID
            if (Platform.CurrentActivity is AndroidX.Activity.ComponentActivity activity)
            {
                ApplySystemBars(activity);
            }
#endif
        }

#if ANDROID
        public static void ApplySystemBars(AndroidX.Activity.ComponentActivity activity)
        {
            SystemBars.Apply(activity, IsDark, Microsoft.Maui.Platform.ColorExtensions.ToPlatform(Get("Surface")));
        }
#endif
    }
}
