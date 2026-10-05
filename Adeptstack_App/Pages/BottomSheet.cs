using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.Shapes;

namespace Adeptstack_App.Pages;

/// <summary>
/// Grundgerüst für Bottom Sheets auf Android und iOS: transparentes Modal über der aktuellen Seite,
/// abgedunkelter Hintergrund, Griff, Einfahr-Animation und nach unten Wischen zum Schließen.
/// Unterklassen setzen nur <see cref="SheetContent"/>.
/// </summary>
public class BottomSheet : ContentPage
{
    // Ab diesem Anteil der Sheet-Höhe schließt das Loslassen das Sheet, darunter schnappt es zurück.
    private const double DragDismissThreshold = 0.25;

    private readonly BoxView _backdrop;
    private readonly Border _sheet;
    private readonly ContentView _body;
    private double _dragOffset;

    public BottomSheet()
    {
        BackgroundColor = Colors.Transparent;
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);

        _backdrop = new BoxView { Color = Color.FromArgb("#99020617"), Opacity = 0 };
        var backdropTap = new TapGestureRecognizer();
        backdropTap.Tapped += async (s, e) => await CloseAsync();
        _backdrop.GestureRecognizers.Add(backdropTap);

        _body = new ContentView();

        // TranslationY startet unterhalb des Bildschirms, OnAppearing fährt das Sheet hoch
        _sheet = new Border
        {
            TranslationY = 600,
            BackgroundColor = Color.FromArgb("#0f172a"),
            Stroke = Color.FromArgb("#1e293b"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20, 20, 0, 0) },
            Padding = new Thickness(16, 10, 16, 28),
            Content = new VerticalStackLayout
            {
                Children =
                {
                    // Griff
                    new BoxView
                    {
                        WidthRequest = 36,
                        HeightRequest = 4,
                        CornerRadius = 2,
                        Color = Color.FromArgb("#334155"),
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 0, 14)
                    },
                    _body
                }
            }
        };

        // Die Ziehgeste liegt auf diesem feststehenden Container und nicht auf dem Sheet selbst:
        // bewegt sich das Element unter dem Finger mit, springen die gemessenen Werte auf Android hin und her.
        var dragArea = new Grid
        {
            VerticalOptions = LayoutOptions.End,
            MaximumWidthRequest = 600,
            BackgroundColor = Colors.Transparent,
            Children = { _sheet }
        };
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += Sheet_PanUpdated;
        dragArea.GestureRecognizers.Add(pan);

        Content = new Grid { Children = { _backdrop, dragArea } };
    }

    /// <summary>
    /// Inhalt unter dem Griff.
    /// </summary>
    public View SheetContent
    {
        get => _body.Content;
        set => _body.Content = value;
    }

    protected bool IsClosing { get; private set; }

    /// <returns>false, wenn schon ein Sheet offen ist. Ein zweiter Long-Press soll kein weiteres stapeln.</returns>
    protected static async Task<bool> ShowAsync(BottomSheet sheet)
    {
        INavigation navigation = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;

        if (navigation == null || navigation.ModalStack.Any(p => p is BottomSheet))
        {
            return false;
        }

        await navigation.PushModalAsync(sheet, false);
        return true;
    }

    /// <summary>
    /// Wird nach jedem Schließen aufgerufen, egal ob per Auswahl, Hintergrund, Zurück-Taste oder Wischen.
    /// </summary>
    protected virtual void OnClosed()
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.WhenAll(
            _backdrop.FadeToAsync(1, 200),
            _sheet.TranslateToAsync(0, 0, 250, Easing.CubicOut));
    }

    protected override bool OnBackButtonPressed()
    {
        _ = CloseAsync();
        return true;
    }

    protected async Task CloseAsync()
    {
        if (IsClosing)
        {
            return;
        }

        IsClosing = true;

        await Task.WhenAll(
            _backdrop.FadeToAsync(0, 150),
            _sheet.TranslateToAsync(0, _sheet.Height, 200, Easing.CubicIn));

        await Navigation.PopModalAsync(false);
        OnClosed();
    }

    private async void Sheet_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (IsClosing || _sheet.Height <= 0)
        {
            return;
        }

        switch (e.StatusType)
        {
            case GestureStatus.Running:
                // Nur nach unten, nach oben bleibt das Sheet an seinem Platz.
                _dragOffset = Math.Max(0, e.TotalY);
                _sheet.TranslationY = _dragOffset;
                _backdrop.Opacity = 1 - Math.Min(1, _dragOffset / _sheet.Height);
                break;

            // Completed liefert kein TotalY mehr, deshalb zählt der letzte Wert aus Running.
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (_dragOffset > _sheet.Height * DragDismissThreshold)
                {
                    await CloseAsync();
                }
                else
                {
                    await Task.WhenAll(
                        _backdrop.FadeToAsync(1, 150),
                        _sheet.TranslateToAsync(0, 0, 150, Easing.CubicOut));
                }

                _dragOffset = 0;
                break;
        }
    }
}
