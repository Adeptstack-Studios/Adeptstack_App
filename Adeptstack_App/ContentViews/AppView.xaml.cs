using Adeptstack_App.ContextClasses;
using AppContext = Adeptstack_App.ContextClasses.AppContext;

namespace Adeptstack_App.ContentViews;

public partial class AppView : ContentView
{
    public static readonly BindableProperty AppProperty = BindableProperty.Create(nameof(App), typeof(AppContext), typeof(AppView), new AppContext());
    public event AppClickedEventArgs AppClicked;

    public AppContext App
    {
        get => (AppContext)GetValue(AppView.AppProperty);
        set => SetValue(AppView.AppProperty, value);
    }

    public AppView()
    {
        InitializeComponent();
    }

    private void clickedOn_Clicked(object sender, EventArgs e)
    {
        AppClicked?.Invoke(this, App);
    }
}