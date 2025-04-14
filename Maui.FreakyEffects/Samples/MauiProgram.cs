using CommunityToolkit.Maui;
using Maui.FreakyControls.Extensions;

namespace Samples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureEffects(effects =>
            {
                //this is throwing an exception: An item with the same key has already been added. Key: Maui.FreakyEffects.TouchEffects.TouchRoutingEffect'
                //effects.InitFreakyEffects();
            });
        builder.InitializeFreakyControls(useFreakyEffects: false);
        return builder.Build();
    }
}