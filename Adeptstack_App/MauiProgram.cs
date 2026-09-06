using Adeptstack_App.ContextClasses;
using Adeptstack_App.Utils;
using Microsoft.Extensions.Logging;
using AppContext = Adeptstack_App.ContextClasses.AppContext;

namespace Adeptstack_App;

public delegate void NewsClickedEventArgs(object sender, NewsContext e);
public delegate void ChangelogClickedEventArgs(object sender, ChangelogContext e);
public delegate void AppClickedEventArgs(object sender, AppContext e);

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Nunito-VariableFont_wght.ttf", "Nunito");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

#if DEBUG
        string oneSignalAppId = "272b665a-e239-4daa-8969-db5e416a8d41"; // Für den Emulator
#else
        string oneSignalAppId = "22db7ac0-b66d-4b83-abf1-5734cb341a5b"; // Für den echten Release
#endif

        PushNotifications.Initialize(oneSignalAppId);

        return builder.Build();
    }
}
