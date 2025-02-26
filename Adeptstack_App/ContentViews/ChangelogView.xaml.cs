namespace Adeptstack_App.ContentViews;

public partial class ChangelogView : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(ChangelogView), string.Empty);
    public static readonly BindableProperty LinkProperty = BindableProperty.Create(nameof(Link), typeof(string), typeof(ChangelogView), string.Empty);
    public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(ChangelogView), string.Empty);
    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(ChangelogView), string.Empty);
    public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(string), typeof(ChangelogView), string.Empty);

    public string Title
    {
        get => (string)GetValue(ChangelogView.TitleProperty);
        set => SetValue(ChangelogView.TitleProperty, value);
    }

    public string Link
    {
        get => (string)GetValue(ChangelogView.LinkProperty);
        set => SetValue(ChangelogView.LinkProperty, value);
    }

    public string Image
    {
        get => (string)GetValue(ChangelogView.ImageProperty);
        set => SetValue(ChangelogView.ImageProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(ChangelogView.DescriptionProperty);
        set => SetValue(ChangelogView.DescriptionProperty, value);
    }

    public string Date
    {
        get => (string)GetValue(ChangelogView.DateProperty);
        set => SetValue(ChangelogView.DateProperty, value);
    }

    public ChangelogView()
    {
        InitializeComponent();
    }
}