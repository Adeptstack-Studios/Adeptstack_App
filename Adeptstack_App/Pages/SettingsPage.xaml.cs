using OneSignalSDK.DotNet;

namespace Adeptstack_App.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        changelogPushNotificationsSwitch.IsToggled = Preferences.Default.Get("WantsChangelogs", false);
    }

    private void changelogPushNotificationsSwitch_Toggled(object sender, ToggledEventArgs e)
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
}