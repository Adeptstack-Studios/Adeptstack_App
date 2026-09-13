namespace Adeptstack_App.Utils
{
    /// <summary>
    /// Kontextmenü (Teilen / Lesezeichen) für News- und Changelog-Karten.
    /// Windows bekommt ein natives Rechtsklick-Menü. MAUI hat keine Long-Press-Geste,
    /// deshalb hängt auf Android, iOS und Mac ein nativer Long-Press am Button und öffnet ein Action Sheet.
    /// </summary>
    static class CardContextMenu
    {
        private const string ShareText = "Share";
        private const string BookmarkText = "Bookmark";
        private const string RemoveBookmarkText = "Remove Bookmark";

        /// <param name="target">Der transparente Button, der über der ganzen Karte liegt.</param>
        public static void Attach(View target, Func<string> getTitle, Func<bool> isBookmarked, Action toggleBookmark, Func<Task<string>> getShareUrl)
        {
            async Task ShareAsync() => await Utilities.ShareAsync(getTitle(), await getShareUrl());

#if WINDOWS
            var shareItem = new MenuFlyoutItem { Text = ShareText, IconImageSource = "share.png" };
            shareItem.Clicked += async (s, e) => await ShareAsync();

            var bookmarkItem = new MenuFlyoutItem { Text = BookmarkText, IconImageSource = "bookmark.png" };
            bookmarkItem.Clicked += (s, e) => toggleBookmark();

            var menu = new MenuFlyout();
            menu.Add(shareItem);
            menu.Add(bookmarkItem);
            FlyoutBase.SetContextFlyout(target, menu);

            // Das Lesezeichen kann sich seit dem Erstellen der Karte geändert haben (z. B. in DisplayContent),
            // deshalb wird der Eintrag erst beim Öffnen des Menüs aktualisiert.
            target.HandlerChanged += (s, e) =>
            {
                if (target.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement element && element.ContextFlyout != null)
                {
                    element.ContextFlyout.Opening += (_, _) =>
                    {
                        bool bookmarked = isBookmarked();
                        bookmarkItem.Text = bookmarked ? RemoveBookmarkText : BookmarkText;
                        bookmarkItem.IconImageSource = bookmarked ? "bookmark_filled.png" : "bookmark.png";
                    };
                }
            };
#elif ANDROID
            target.HandlerChanged += (s, e) =>
            {
                if (target.Handler?.PlatformView is Android.Views.View view)
                {
                    view.LongClick += async (_, args) =>
                    {
                        // Handled verhindert, dass nach dem Loslassen zusätzlich der normale Klick auslöst.
                        args.Handled = true;
                        view.PerformHapticFeedback(Android.Views.FeedbackConstants.LongPress);
                        await ShowActionSheetAsync(getTitle(), isBookmarked, toggleBookmark, ShareAsync);
                    };
                }
            };
#elif IOS || MACCATALYST
            target.HandlerChanged += (s, e) =>
            {
                if (target.Handler?.PlatformView is UIKit.UIView view)
                {
                    view.AddGestureRecognizer(new UIKit.UILongPressGestureRecognizer(async recognizer =>
                    {
                        if (recognizer.State == UIKit.UIGestureRecognizerState.Began)
                        {
                            await ShowActionSheetAsync(getTitle(), isBookmarked, toggleBookmark, ShareAsync);
                        }
                    }));
                }
            };
#endif
        }

        private static async Task ShowActionSheetAsync(string title, Func<bool> isBookmarked, Action toggleBookmark, Func<Task> share)
        {
            Page page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page == null)
            {
                return;
            }

            string bookmarkText = isBookmarked() ? RemoveBookmarkText : BookmarkText;
            string choice = await page.DisplayActionSheetAsync(title, "Cancel", null, ShareText, bookmarkText);

            if (choice == ShareText)
            {
                await share();
            }
            else if (choice == bookmarkText)
            {
                toggleBookmark();
            }
        }
    }
}
