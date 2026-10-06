using Adeptstack_App.ContextClasses;
using Adeptstack_App.Utils;

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

        CardContextMenu.Attach(clickedOn,
            () => Changelog.title,
            () => "Changelog",
            () => Changelog.imageUrl,
            () => Bookmarks.IsBookmarked(Changelog),
            () => Bookmarks.Toggle(Changelog),
            () => Task.FromResult(Utilities.GetChangelogShareText(Changelog)));
    }

    private void clickedOn_Clicked(object sender, EventArgs e)
    {
        ChangelogClicked?.Invoke(this, Changelog);
    }
}
