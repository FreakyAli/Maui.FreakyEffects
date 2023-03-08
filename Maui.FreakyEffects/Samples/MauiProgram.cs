using Microsoft.Extensions.Logging;
using Maui.FreakyEffects;
using Microsoft.Maui.Hosting;
using Maui.FreakyControls.Extensions;

namespace Samples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddFreakyHandlers(); // To Init your freaky handlers for Entry and Editor
            })
            .ConfigureEffects(effects =>
            {
                effects.InitFreakyEffects();
            });

        builder.InitSkiaSharp();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
