namespace Adeptstack_App.Pages;

/// <summary>
/// Bottom Sheet für Hinweise und Bestätigungen statt der normalen Alert-Popups.
/// Auf Windows gibt es wie bei <see cref="SelectionSheet"/> kein Sheet, dort öffnet sich der native Dialog.
/// </summary>
public class MessageSheet : BottomSheet
{
    private readonly TaskCompletionSource<bool> _result = new TaskCompletionSource<bool>();

    private MessageSheet(string title, string message, IReadOnlyList<(string Title, string Subtitle)> items,
        string confirm, string cancel, bool destructive)
    {
        var layout = new VerticalStackLayout
        {
            Spacing = 8,
            Padding = new Thickness(4, 0),
            Children =
            {
                new Label
                {
                    Text = title,
                    TextColor = Color.FromArgb("#f8fafc"),
                    FontFamily = "OpenSansSemibold",
                    FontSize = 18
                }
            }
        };

        if (!string.IsNullOrEmpty(message))
        {
            layout.Add(new Label
            {
                Text = message,
                TextColor = Color.FromArgb("#94a3b8"),
                FontFamily = "OpenSansRegular",
                FontSize = 14,
                LineHeight = 1.2
            });
        }

        if (items?.Count > 0)
        {
            var list = new VerticalStackLayout { Margin = new Thickness(0, 6, 0, 0) };

            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    list.Add(new BoxView { HeightRequest = 1, Color = Color.FromArgb("#1e293b") });
                }

                list.Add(new VerticalStackLayout
                {
                    Padding = new Thickness(0, 10),
                    Spacing = 2,
                    Children =
                    {
                        new Label { Text = items[i].Title, TextColor = Color.FromArgb("#f8fafc"), FontFamily = "OpenSansSemibold", FontSize = 15 },
                        new Label { Text = items[i].Subtitle, TextColor = Color.FromArgb("#94a3b8"), FontFamily = "OpenSansRegular", FontSize = 13 }
                    }
                });
            }

            // Lange Listen scrollen, statt das Sheet über den Bildschirm hinaus wachsen zu lassen
            layout.Add(new ScrollView { Content = list, MaximumHeightRequest = 360 });
        }

        var buttons = new Grid { ColumnSpacing = 10, Margin = new Thickness(0, 16, 0, 0) };

        if (cancel != null)
        {
            buttons.ColumnDefinitions = new ColumnDefinitionCollection(new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star));
            buttons.Add(CreateButton(cancel, Color.FromArgb("#1e293b"), Color.FromArgb("#f8fafc"), false));
        }

        var confirmButton = destructive
            ? CreateButton(confirm, Color.FromArgb("#dc2626"), Colors.White, true)
            : CreateButton(confirm, Color.FromArgb("#3b82f6"), Colors.White, true);
        buttons.Add(confirmButton, cancel != null ? 1 : 0);

        layout.Add(buttons);
        SheetContent = layout;
    }

    private Button CreateButton(string text, Color background, Color textColor, bool result)
    {
        var button = new Button
        {
            Text = text.ToUpper(),
            FontFamily = "OpenSansSemibold",
            FontSize = 13,
            CornerRadius = 22,
            HeightRequest = 44,
            BackgroundColor = background,
            TextColor = textColor
        };
        button.Clicked += async (s, e) => await CloseWithResultAsync(result);
        return button;
    }

    /// <param name="cancel">null: nur ein Button, z. B. "OK" bei einem Hinweis.</param>
    /// <param name="destructive">Bestätigung in Rot, z. B. fürs Löschen.</param>
    /// <param name="items">Optionale Liste unter dem Text, jeweils Titel und Untertitel.</param>
    /// <returns>true, wenn bestätigt wurde. Schließen per Hintergrund, Wischen oder Zurück zählt als Abbruch.</returns>
    public static async Task<bool> ShowAsync(Page page, string title, string message, string confirm = "OK", string cancel = null,
        bool destructive = false, IReadOnlyList<(string Title, string Subtitle)> items = null)
    {
#if WINDOWS
        string text = message ?? "";

        if (items?.Count > 0)
        {
            string list = string.Join("\n\n", items.Select(i => $"{i.Title}\n{i.Subtitle}"));
            text = string.IsNullOrEmpty(text) ? list : $"{text}\n\n{list}";
        }

        if (cancel == null)
        {
            await page.DisplayAlertAsync(title, text, confirm);
            return true;
        }

        return await page.DisplayAlertAsync(title, text, confirm, cancel);
#else
        var sheet = new MessageSheet(title, message, items, confirm, cancel, destructive);

        if (!await BottomSheet.ShowAsync(sheet))
        {
            return false;
        }

        return await sheet._result.Task;
#endif
    }

    private async Task CloseWithResultAsync(bool result)
    {
        if (IsClosing)
        {
            return;
        }

        _result.TrySetResult(result);
        await CloseAsync();
    }

    protected override void OnClosed()
    {
        _result.TrySetResult(false);
    }
}
