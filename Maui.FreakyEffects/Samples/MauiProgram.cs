using CommunityToolkit.Maui;
using Maui.FreakyControls.Extensions;
using Maui.FreakyEffects;
using Microsoft.Extensions.Logging;

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
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}