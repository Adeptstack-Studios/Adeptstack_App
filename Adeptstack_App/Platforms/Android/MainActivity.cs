using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity;
using Microsoft.Maui;

namespace Adeptstack_App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Unspecified)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		// Randlose Anzeige auch unter Android < 15 wie ab SDK 35. Die App ist immer dunkel,
		// daher transparente Systemleisten mit hellen Symbolen.
		var barStyle = SystemBarStyle.Dark(Android.Graphics.Color.Transparent);
		EdgeToEdge.Enable(this, barStyle, barStyle);

		base.OnCreate(savedInstanceState);
	}
}
