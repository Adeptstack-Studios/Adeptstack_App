namespace Adeptstack_App.Pages;

/// <summary>
/// Bottom Sheet zur Auswahl einer Option, z. B. der Sortierung.
/// Auf Windows gibt es wie beim Kartenmenü kein Sheet, dort öffnet sich der native Dialog.
/// </summary>
public class SelectionSheet : BottomSheet
{
    private readonly TaskCompletionSource<int> _result = new TaskCompletionSource<int>();

    private SelectionSheet(string title, IReadOnlyList<string> options, int selectedIndex)
    {
        var layout = new VerticalStackLayout
        {
            Children =
            {
                new Label
                {
                    Text = title,
                    TextColor = Color.FromArgb("#3b82f6"),
                    FontFamily = "OpenSansSemibold",
                    FontSize = 11,
                    TextTransform = TextTransform.Uppercase,
                    CharacterSpacing = 1,
                    Padding = new Thickness(4, 0, 4, 14)
                },
                new BoxView { HeightRequest = 1, Color = Color.FromArgb("#1e293b") }
            }
        };

        for (int i = 0; i < options.Count; i++)
        {
            int index = i;
            bool isSelected = i == selectedIndex;

            // Transparenter Hintergrund, damit die ganze Zeile den Tap bekommt
            var row = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                Padding = new Thickness(4, i == 0 ? 14 : 10, 4, 10),
                BackgroundColor = Colors.Transparent
            };

            row.Add(new Label
            {
                Text = options[i],
                TextColor = isSelected ? Color.FromArgb("#3b82f6") : Color.FromArgb("#f8fafc"),
                FontFamily = isSelected ? "OpenSansSemibold" : "OpenSansRegular",
                FontSize = 15,
                VerticalOptions = LayoutOptions.Center
            });

            row.Add(new Label
            {
                Text = "✓",
                IsVisible = isSelected,
                TextColor = Color.FromArgb("#3b82f6"),
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center
            }, 1);

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => await SelectAsync(index);
            row.GestureRecognizers.Add(tap);

            layout.Add(row);
        }

        SheetContent = layout;
    }

    /// <returns>Index der gewählten Option oder -1 bei Abbruch.</returns>
    public static async Task<int> PickAsync(Page page, string title, IReadOnlyList<string> options, int selectedIndex)
    {
#if WINDOWS
        string[] labels = options.Select((o, i) => i == selectedIndex ? $"✓ {o}" : o).ToArray();
        string choice = await page.DisplayActionSheetAsync(title, "Cancel", null, labels);
        return Array.IndexOf(labels, choice);
#else
        var sheet = new SelectionSheet(title, options, selectedIndex);

        if (!await ShowAsync(sheet))
        {
            return -1;
        }

        return await sheet._result.Task;
#endif
    }

    private async Task SelectAsync(int index)
    {
        if (IsClosing)
        {
            return;
        }

        _result.TrySetResult(index);
        await CloseAsync();
    }

    protected override void OnClosed()
    {
        _result.TrySetResult(-1);
    }
}
