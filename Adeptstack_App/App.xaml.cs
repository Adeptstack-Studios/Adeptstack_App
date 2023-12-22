namespace Adeptstack_App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Application.Current.UserAppTheme = AppTheme.Dark;
        MainPage = new AppShell();
    }
}
