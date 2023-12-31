using CommunityToolkit.Maui;
using Maui.FreakyEffects;
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
                effects.InitFreakyEffects();
            });

        builder.InitializeFreakyControls();
        return builder.Build();
    }
}