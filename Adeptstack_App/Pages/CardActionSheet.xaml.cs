using Adeptstack_App.Utils;

namespace Adeptstack_App.Pages;

/// <summary>
/// Bottom Sheet für das Long-Press-Menü der Karten auf Android und iOS.
/// </summary>
public partial class CardActionSheet : BottomSheet
{
    private readonly Action _toggleBookmark;
    private readonly Func<Task> _share;

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

    public static Task ShowAsync(string title, string caption, string imageUrl, Func<bool> isBookmarked, Action toggleBookmark, Func<Task> share)
    {
        return ShowAsync(new CardActionSheet(title, caption, imageUrl, isBookmarked(), toggleBookmark, share));
    }

    private async void Share_Tapped(object sender, TappedEventArgs e)
    {
        if (IsClosing)
        {
            return;
        }

        // Erst schließen, sonst öffnet sich der System-Teilen-Dialog hinter dem Sheet.
        await CloseAsync();
        await _share();
    }

    private async void Bookmark_Tapped(object sender, TappedEventArgs e)
    {
        if (IsClosing)
        {
            return;
        }

        _toggleBookmark();
        await CloseAsync();
    }
}
