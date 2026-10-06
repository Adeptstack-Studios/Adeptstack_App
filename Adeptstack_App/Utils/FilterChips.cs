using Adeptstack_App.ContextClasses;

namespace Adeptstack_App.Utils
{
    /// <summary>
    /// Filter-Chips für Kategorien, Apps und Channels.
    /// </summary>
    static class FilterChips
    {
        /// <param name="Value">Wert für die API. null steht für den "All"-Chip.</param>
        public record Chip(string Value, string Label);

        /// <summary>
        /// Chip mit Anzahl, z. B. "DEVLOG  5". Ohne Anzahl nur der Name.
        /// </summary>
        public static Chip FromOption(FilterOption option)
        {
            string label = option.count > 0 ? $"{option.name.ToUpper()}  {option.count}" : option.name.ToUpper();
            return new Chip(option.name, label);
        }

        /// <summary>
        /// "All"-Chip, die Anzahl ist die Summe aller Optionen.
        /// </summary>
        public static Chip All(IEnumerable<FilterOption> options)
        {
            return FromOption(new FilterOption { name = "All", count = options.Sum(o => o.count) }) with { Value = null };
        }

        /// <param name="selected">Wird nur bei einem Wechsel aufgerufen, nicht beim Tippen auf den aktiven Chip.</param>
        public static void Build(HorizontalStackLayout layout, IEnumerable<Chip> chips, string selectedValue, Action<string> selected)
        {
            layout.Children.Clear();

            foreach (var chip in chips)
            {
                bool isSelected = string.Equals(chip.Value, selectedValue, StringComparison.OrdinalIgnoreCase);

                var btn = new Button
                {
                    Text = chip.Label,
                    FontFamily = "OpenSansSemibold",
                    FontSize = 12,
                    CornerRadius = 20,
                    HeightRequest = 36,
                    Padding = new Thickness(16, 0),
                    BackgroundColor = isSelected ? Color.FromArgb("#3b82f6") : Color.FromArgb("#1e293b"),
                    TextColor = isSelected ? Colors.White : Color.FromArgb("#94a3b8")
                };

                btn.Clicked += (s, e) =>
                {
                    if (!isSelected)
                    {
                        selected(chip.Value);
                    }
                };

                layout.Children.Add(btn);
            }
        }
    }
}
