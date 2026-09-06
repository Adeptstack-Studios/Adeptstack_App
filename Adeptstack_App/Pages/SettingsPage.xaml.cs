using Adeptstack_App.Net;
using Adeptstack_App.Utils;

namespace Adeptstack_App.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

        // Ohne OneSignal (Windows, Mac) gibt es nichts zu schalten.
        notificationsSection.IsVisible = PushNotifications.IsSupported;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (PushNotifications.IsSupported)
        {
            changelogPushNotificationsSwitch.IsToggled = PushNotifications.WantsChangelogs();
        }
    }

    private async void changelogPushNotificationsSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (await Web.IsConnectedToInternetAsync())
        {
            PushNotifications.SetWantsChangelogs(e.Value);
        }
        else
        {
            await DisplayAlertAsync("No Internet Connection", "You need an internet connection to change this setting.", "OK");
        }
    }

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