namespace Adeptstack_App;

public partial class MainPage : ContentPage
{
    string url = "https://app-adeptstack.vercel.app/News";
    public MainPage()
    {
        InitializeComponent();
        web.Source = url;
    }

    private async void web_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (Utilities.IsConnectedToInternet())
        {
            if (url != e.Url)
            {
                e.Cancel = true;
                await Navigation.PushAsync(new DisplayContent(e.Url));
            }
        }
        //else
        //{
        //    await Navigation.PushAsync(new NoWifi());
        //}
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

