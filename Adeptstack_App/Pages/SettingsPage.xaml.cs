using Adeptstack_App.Net;
using Adeptstack_App.Utils;

namespace Adeptstack_App.Pages;

public partial class SettingsPage : ContentPage
{
    // Die Adeptstack App selbst in der API, für "What's New"
    private const int OwnAppId = 5;
    private const string FeedbackEmail = "support@adeptstack.net";
    private const string PlayStoreUrl = "https://play.google.com/store/apps/details?id=com.adeptstack.adeptstack_app";
    private const string MicrosoftStoreReviewUrl = "ms-windows-store://review/?ProductId=9PKTGV1P8NHD";

    private static readonly (string Name, string License)[] Licenses =
    {
        (".NET MAUI", "MIT License"),
        ("Markdig", "BSD 2-Clause License"),
        ("OneSignal .NET SDK", "Modified MIT License"),
        ("System.Text.Json", "MIT License"),
        ("Nunito Font", "SIL Open Font License 1.1"),
        ("Open Sans Font", "SIL Open Font License 1.1")
    };

    // Beim Setzen der Schalter aus dem Code keine Toggled-Logik auslösen
    private bool _isLoading;

    public SettingsPage()
    {
        InitializeComponent();

        // Ohne OneSignal (Windows, Mac) gibt es nichts zu schalten.
        notificationsSection.IsVisible = PushNotifications.IsSupported;

        // Auf dem Desktop gibt es keinen In-App-Browser, dort öffnet sich immer der Standardbrowser.
        openLinksInAppRow.IsVisible = DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS;

        // Die App steht (noch) nicht im App Store
        rateRow.IsVisible = DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.WinUI;

        // Windows liefert die Paketversion (2.5.0.0) und als Build nur die Revision, die immer 0 ist
        versionLabel.Text = DeviceInfo.Platform == DevicePlatform.WinUI
            ? $"Version {AppInfo.Version.ToString(3)}"
            : $"Version {AppInfo.VersionString} (Build {AppInfo.BuildString})";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _isLoading = true;

        if (PushNotifications.IsSupported)
        {
            changelogPushNotificationsSwitch.IsToggled = PushNotifications.WantsChangelogs();
        }

        openLinksInAppSwitch.IsToggled = AppSettings.OpenLinksInApp;
        rememberFiltersSwitch.IsToggled = AppSettings.RememberFilters;
        dataSaverSwitch.IsToggled = AppSettings.DataSaver;

        _isLoading = false;

        textSizeLabel.Text = AppSettings.TextSizes[AppSettings.TextSizeIndex].Label;
        UpdatePermissionWarning();
        UpdateBookmarkCount();

        // Zurück aus den Systemeinstellungen: Berechtigung kann sich geändert haben
        if (Window != null)
        {
            Window.Resumed += Window_Resumed;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (Window != null)
        {
            Window.Resumed -= Window_Resumed;
        }
    }

    private void Window_Resumed(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(UpdatePermissionWarning);
    }

    // --- Benachrichtigungen ---

    private void UpdatePermissionWarning()
    {
        permissionWarning.IsVisible = PushNotifications.IsSupported && !PushNotifications.HasPermission;
    }

    private async void changelogPushNotificationsSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (_isLoading)
        {
            return;
        }

        if (await Web.IsConnectedToInternetAsync())
        {
            PushNotifications.SetWantsChangelogs(e.Value);
        }
        else
        {
            // Nicht gespeichert, also auch den Schalter zurückstellen
            _isLoading = true;
            changelogPushNotificationsSwitch.IsToggled = !e.Value;
            _isLoading = false;

            await MessageSheet.ShowAsync(this, "No Internet Connection", "You need an internet connection to change this setting.");
        }
    }

    private void OpenSystemSettings_Clicked(object sender, EventArgs e)
    {
        AppInfo.Current.ShowSettingsUI();
    }

    // --- Lesen ---

    private async void TextSize_Tapped(object sender, TappedEventArgs e)
    {
        var options = AppSettings.TextSizes.Select(s => s.Label).ToList();
        int index = await SelectionSheet.PickAsync(this, "Text size", options, AppSettings.TextSizeIndex);

        if (index < 0)
        {
            return;
        }

        AppSettings.TextSizeIndex = index;
        textSizeLabel.Text = options[index];
    }

    private void openLinksInAppSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (!_isLoading)
        {
            AppSettings.OpenLinksInApp = e.Value;
        }
    }

    private void rememberFiltersSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (!_isLoading)
        {
            AppSettings.RememberFilters = e.Value;
        }
    }

    // --- Daten und Speicher ---

    private void dataSaverSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (!_isLoading)
        {
            AppSettings.DataSaver = e.Value;
        }
    }

    private void UpdateBookmarkCount()
    {
        int count = Bookmarks.Count;
        bookmarkCountLabel.Text = count switch
        {
            0 => "No saved bookmarks.",
            1 => "1 saved bookmark, available offline.",
            _ => $"{count} saved bookmarks, available offline."
        };
        clearBookmarksButton.IsVisible = count > 0;
    }

    private async void ClearBookmarks_Clicked(object sender, EventArgs e)
    {
        bool confirmed = await MessageSheet.ShowAsync(this, "Remove All Bookmarks?",
            "All saved articles and changelogs will be removed from this device. This can't be undone.",
            "Remove", "Cancel", destructive: true);

        if (confirmed)
        {
            Bookmarks.Clear();
            UpdateBookmarkCount();
        }
    }

    // --- Über die App ---

    private async void WhatsNew_Tapped(object sender, TappedEventArgs e)
    {
        if (!await Web.IsConnectedToInternetAsync())
        {
            await MessageSheet.ShowAsync(this, "No Internet Connection", "You need an internet connection to see what's new.");
            return;
        }

        var app = await Task.Run(() =>
        {
            try
            {
                return Web.GetAppById(OwnAppId);
            }
            catch
            {
                return null;
            }
        });

        if (app == null)
        {
            await MessageSheet.ShowAsync(this, "Something Went Wrong", "The changelog couldn't be loaded. Please try again later.");
            return;
        }

        await Navigation.PushAsync(new AppChangelog(app));
    }

    private async void Feedback_Tapped(object sender, TappedEventArgs e)
    {
        // Version und Plattform gleich mitschicken, das spart Rückfragen
        string subject = Uri.EscapeDataString($"Adeptstack App Feedback (v{AppInfo.VersionString}, {DeviceInfo.Platform} {DeviceInfo.VersionString})");

        try
        {
            await Launcher.OpenAsync($"mailto:{FeedbackEmail}?subject={subject}");
        }
        catch
        {
            await MessageSheet.ShowAsync(this, "No Mail App", $"You can reach us at {FeedbackEmail}.");
        }
    }

    private async void Rate_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                await Launcher.OpenAsync(MicrosoftStoreReviewUrl);
            }
            else
            {
                // Mit installiertem Play Store öffnet sich direkt die App, sonst der Browser
                await Launcher.OpenAsync(PlayStoreUrl);
            }
        }
        catch
        {
        }
    }

    private async void Licenses_Tapped(object sender, TappedEventArgs e)
    {
        await MessageSheet.ShowAsync(this, "Open Source Licenses", "This app is built with these libraries and fonts.",
            items: Licenses.Select(l => (l.Name, l.License)).ToList());
    }

    // --- Rechtliches ---

    private void TermsOfUse_Tapped(object sender, TappedEventArgs e)
    {
        Launcher.OpenAsync("https://www.adeptstack.net/terms");
    }

    private void Privacy_Tapped(object sender, TappedEventArgs e)
    {
        Launcher.OpenAsync("https://www.adeptstack.net/privacy");
    }

    private void Impressum_Tapped(object sender, TappedEventArgs e)
    {
        Launcher.OpenAsync("https://www.adeptstack.net/imprint");
    }
}
