using Maui.FreakyEffects.SkiaScene;
using Maui.FreakyEffects.SkiaScene.TouchManipulation;
using Maui.FreakyEffects.TouchTracking;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Samples;

public partial class MainPage : FreakyBaseContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }

    async void ListView_ItemTapped(System.Object sender, Microsoft.Maui.Controls.ItemTappedEventArgs e)
    {
        string route = e.Item.ToString();
        await Shell.Current.GoToAsync(route);
    }
}