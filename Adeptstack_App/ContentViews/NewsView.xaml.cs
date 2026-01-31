using Adeptstack_App.ContextClasses;

namespace Adeptstack_App.ContentViews;

public partial class NewsView : ContentView
{

    public static readonly BindableProperty NewsProperty = BindableProperty.Create(nameof(News), typeof(NewsContext), typeof(NewsView), new NewsContext());
    public event NewsClickedEventArgs NewsClicked;

    public NewsContext News
    {
        get => (NewsContext)GetValue(NewsView.NewsProperty);
        set => SetValue(NewsView.NewsProperty, value);
    }

    public NewsView()
    {
        InitializeComponent();
    }

    private void clickedOn_Clicked(object sender, EventArgs e)
    {
        NewsClicked?.Invoke(this, News);
    }
}