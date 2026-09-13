using Adeptstack_App.Utils;

namespace Adeptstack_App.Pages;

/// <summary>
/// Bottom Sheet für das Long-Press-Menü der Karten auf Android und iOS.
/// Wird als transparentes Modal über die aktuelle Seite gelegt.
/// </summary>
public partial class CardActionSheet : ContentPage
{
    private readonly Action _toggleBookmark;
    private readonly Func<Task> _share;
    private bool _closing;

    private CardActionSheet(string title, string caption, string imageUrl, bool bookmarked, Action toggleBookmark, Func<Task> share)
    {
        InitializeComponent();

        _toggleBookmark = toggleBookmark;
        _share = share;

        titleLabel.Text = title;
        captionLabel.Text = caption;
        captionLabel.IsVisible = !string.IsNullOrWhiteSpace(caption);
        previewImageFrame.IsVisible = !string.IsNullOrWhiteSpace(imageUrl);
        previewImage.Source = imageUrl;

        shareLabel.Text = CardContextMenu.ShareText;
        bookmarkLabel.Text = bookmarked ? CardContextMenu.RemoveBookmarkText : CardContextMenu.BookmarkText;
        bookmarkIcon.Source = bookmarked ? "bookmark_filled.png" : "bookmark.png";
    }

    public static async Task ShowAsync(string title, string caption, string imageUrl, Func<bool> isBookmarked, Action toggleBookmark, Func<Task> share)
    {
        INavigation navigation = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;

        // Ein zweiter Long-Press, während das Sheet noch offen ist, soll kein weiteres stapeln.
        if (navigation == null || navigation.ModalStack.Any(p => p is CardActionSheet))
        {
            return;
        }

        await navigation.PushModalAsync(new CardActionSheet(title, caption, imageUrl, isBookmarked(), toggleBookmark, share), false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.WhenAll(
            backdrop.FadeToAsync(1, 200),
            sheet.TranslateToAsync(0, 0, 250, Easing.CubicOut));
    }

    protected override bool OnBackButtonPressed()
    {
        _ = CloseAsync();
        return true;
    }

    private async Task CloseAsync()
    {
        if (_closing)
        {
            return;
        }

        _closing = true;

        await Task.WhenAll(
            backdrop.FadeToAsync(0, 150),
            sheet.TranslateToAsync(0, sheet.Height, 200, Easing.CubicIn));

        await Navigation.PopModalAsync(false);
    }

    private async void Backdrop_Tapped(object sender, TappedEventArgs e)
    {
        await CloseAsync();
    }

    private async void Share_Tapped(object sender, TappedEventArgs e)
    {
        if (_closing)
        {
            return;
        }

        // Erst schließen, sonst öffnet sich der System-Teilen-Dialog hinter dem Sheet.
        await CloseAsync();
        await _share();
    }

    private async void Bookmark_Tapped(object sender, TappedEventArgs e)
    {
        if (_closing)
        {
            return;
        }

        _toggleBookmark();
        await CloseAsync();
    }
}
