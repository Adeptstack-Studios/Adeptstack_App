using Android.App;
using Android.Content.PM;
using Android.OS;
using Adeptstack_App.Utils;
using Microsoft.Maui;

namespace Adeptstack_App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Unspecified)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		// Randlose Anzeige auch unter Android < 15 wie ab SDK 35. Die App steht hier noch nicht,
		// daher erst dunkel und nach dem Start passend zum Theme.
		SystemBars.Apply(this, true, new Android.Graphics.Color(0x0f, 0x17, 0x2a));

		base.OnCreate(savedInstanceState);

		AppTheming.ApplySystemBars(this);
	}
}
