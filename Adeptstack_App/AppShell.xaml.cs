namespace Adeptstack_App;

public partial class AppShell : Shell
{
    private static readonly (string Title, string Icon, string Route, Type Page)[] Sections =
    {
        ("News",       "news.png",     "MainPage",   typeof(MainPage)),
        ("Changelogs", "changes.png",  "Changelogs", typeof(Changelogs)),
        ("Settings",   "settings.png", "Settings",   typeof(Pages.SettingsPage)),
    };

    public AppShell()
    {
        InitializeComponent();
        BuildNavigation();
    }

    /// <summary>
    /// Android und iOS behalten die gewohnte TabBar am unteren Rand. Auf dem Desktop
    /// ist die deplatziert, deshalb bekommt Windows stattdessen ein Flyout mit einem
    /// eigenen Eintrag pro Seite.
    /// </summary>
    private void BuildNavigation()
    {
#if WINDOWS
        foreach (var section in Sections)
        {
            var flyoutItem = new FlyoutItem { Title = section.Title, Icon = section.Icon };
            flyoutItem.Items.Add(CreateContent(section));
            Items.Add(flyoutItem);
        }

        FlyoutBehavior = FlyoutBehavior.Flyout;
#else
        var tabBar = new TabBar();

        foreach (var section in Sections)
        {
            tabBar.Items.Add(CreateContent(section));
        }

        Items.Add(tabBar);
        FlyoutBehavior = FlyoutBehavior.Disabled;
#endif
    }

    private static ShellContent CreateContent((string Title, string Icon, string Route, Type Page) section)
    {
        return new ShellContent
        {
            Title = section.Title,
            Icon = section.Icon,
            Route = section.Route,
            ContentTemplate = new DataTemplate(section.Page)
        };
    }
}
