using Adeptstack_App.ContextClasses;
using Adeptstack_App.Net;
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

    /// <summary>
    /// Slug der App für den Share-Link. Fehlt er (z. B. auf der Bookmarks-Seite), wird er beim Teilen von der API geholt.
    /// </summary>
    public string AppSlug { get; set; }

    public ChangelogView()
    {
        InitializeComponent();

        CardContextMenu.Attach(clickedOn,
            () => Changelog.title,
            () => Bookmarks.IsBookmarked(Changelog),
            () => Bookmarks.Toggle(Changelog),
            GetShareTextAsync);
    }

    private async Task<string> GetShareTextAsync()
    {
        AppSlug ??= await Task.Run(() => Web.GetAppById(Changelog.appId)?.slug);
        return Utilities.GetChangelogShareText(Changelog, Utilities.GetChangelogUrl(AppSlug));
    }

    private void clickedOn_Clicked(object sender, EventArgs e)
    {
        ChangelogClicked?.Invoke(this, Changelog);
    }
}
