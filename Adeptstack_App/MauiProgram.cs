using Adeptstack_App.ContextClasses;
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

        return builder.Build();
    }
}
