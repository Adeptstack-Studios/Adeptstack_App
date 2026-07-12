using Adeptstack_App.Net;
using OneSignalSDK.DotNet;

namespace Adeptstack_App.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        changelogPushNotificationsSwitch.IsToggled = GetChangelogTagFromOneSignal();
    }

    public bool GetChangelogTagFromOneSignal()
    {
        IDictionary<string, string> tags = OneSignal.User.GetTags();

        if (tags != null && tags.TryGetValue("WantsChangelogs", out string tagValue))
        {
            return tagValue.ToLower() == "true";
        }
        return true;
    }

    private async void changelogPushNotificationsSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (await Web.IsConnectedToInternetAsync())
        {
            bool isEnabled = e.Value;

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