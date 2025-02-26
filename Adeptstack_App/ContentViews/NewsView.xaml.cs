namespace Adeptstack_App.ContentViews;

public partial class NewsView : ContentView
{

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(NewsView), string.Empty);
    public static readonly BindableProperty LinkProperty = BindableProperty.Create(nameof(Link), typeof(string), typeof(NewsView), string.Empty);
    public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(NewsView), string.Empty);
    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(NewsView), string.Empty);
    public static readonly BindableProperty CategoryProperty = BindableProperty.Create(nameof(Category), typeof(string), typeof(NewsView), string.Empty);
    public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(string), typeof(NewsView), string.Empty);

    public string Title
    {
        get => (string)GetValue(NewsView.TitleProperty);
        set => SetValue(NewsView.TitleProperty, value);
    }

    public string Link
    {
        get => (string)GetValue(NewsView.LinkProperty);
        set => SetValue(NewsView.LinkProperty, value);
    }

    public string Image
    {
        get => (string)GetValue(NewsView.ImageProperty);
        set => SetValue(NewsView.ImageProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(NewsView.DescriptionProperty);
        set => SetValue(NewsView.DescriptionProperty, value);
    }

    public string Category
    {
        get => (string)GetValue(NewsView.CategoryProperty);
        set => SetValue(NewsView.CategoryProperty, value);
    }

    public string Date
    {
        get => (string)GetValue(NewsView.DateProperty);
        set => SetValue(NewsView.DateProperty, value);
    }

    public NewsView()
    {
        InitializeComponent();
    }
}