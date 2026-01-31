using Android.App;
using Android.Content.PM;
using Microsoft.Maui;

namespace Adeptstack_App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Unspecified)]
public class MainActivity : MauiAppCompatActivity
{
}
