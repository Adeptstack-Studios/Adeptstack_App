using AndroidX.Activity;

namespace Adeptstack_App;

/// <summary>
/// Randlose Anzeige mit transparenten Systemleisten. Die Symbole passen sich dem Theme der App an.
/// </summary>
static class SystemBars
{
	/// <param name="background">Farbe hinter der Statusleiste, dort scheint der Fensterhintergrund durch.</param>
	public static void Apply(ComponentActivity activity, bool dark, Android.Graphics.Color background)
	{
		int transparent = Android.Graphics.Color.Transparent;
		var style = dark
			? SystemBarStyle.Dark(transparent)
			: SystemBarStyle.Light(transparent, transparent);

		EdgeToEdge.Enable(activity, style, style);
		activity.Window?.DecorView.SetBackgroundColor(background);
	}
}
