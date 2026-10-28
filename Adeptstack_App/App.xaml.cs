using Adeptstack_App.Utils;
using Microsoft.Maui.ApplicationModel;
namespace Adeptstack_App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        AppTheming.Initialize(this);
        MainPage = new AppShell();
    }
}
