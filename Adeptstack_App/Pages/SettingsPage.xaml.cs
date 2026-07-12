using Adeptstack_App.Net;
using OneSignalSDK.DotNet;

namespace Adeptstack_App.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        changelogPushNotificationsSwitch.IsToggled = Preferences.Default.Get("WantsChangelogs", true);
    }

    private async Task changelogPushNotificationsSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (Web.IsConnectedToInternetAsync().Result)
        {
            bool isEnabled = e.Value;

            Preferences.Default.Set("WantsChangelogs", isEnabled);

            if (isEnabled)
            {
                OneSignal.User.AddTag("WantsChangelogs", "true");
            }
            else
            {
                OneSignal.User.RemoveTag("WantsChangelogs");
            } 
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