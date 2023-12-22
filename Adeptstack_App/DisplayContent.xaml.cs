namespace Adeptstack_App;

public partial class DisplayContent : ContentPage
{
    string link = "";
    public DisplayContent(string url)
    {
        InitializeComponent();
        web.Source = url;
        this.Title = Utilities.GetTitle(url);
        this.link = url;

    }

    private void web_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url != link)
        {
            e.Cancel = true;
            Browser.Default.OpenAsync(e.Url).Wait();
        }
    }

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
        RefreshView rfv = sender as RefreshView;

        if (rfv.IsRefreshing)
        {
            if (Utilities.IsConnectedToInternet())
            {
                web.Reload();
                rfv.IsRefreshing = false;
            }
            rfv.IsRefreshing = false;
        }
    }
}