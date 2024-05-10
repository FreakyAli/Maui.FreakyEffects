namespace Samples;

public partial class MainPage : FreakyBaseContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }

    private async void ListView_ItemTapped(System.Object sender, Microsoft.Maui.Controls.ItemTappedEventArgs e)
    {
        string route = e.Item.ToString();
        await Shell.Current.GoToAsync(route);
    }
}