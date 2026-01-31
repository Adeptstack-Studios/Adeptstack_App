using Adeptstack_App.ContextClasses;

namespace Adeptstack_App.ContentViews;

public partial class ChangelogView : ContentView
{
    public static readonly BindableProperty ChangelogProperty = BindableProperty.Create(nameof(Changelog), typeof(ChangelogContext), typeof(ChangelogView), new ChangelogContext());
    public event ChangelogClickedEventArgs ChangelogClicked;

    public ChangelogContext Changelog
    {
        get => (ChangelogContext)GetValue(ChangelogView.ChangelogProperty);
        set => SetValue(ChangelogView.ChangelogProperty, value);
    }

    public ChangelogView()
    {
        InitializeComponent();
    }

    private void clickedOn_Clicked(object sender, EventArgs e)
    {
        ChangelogClicked?.Invoke(this, Changelog);
    }
}