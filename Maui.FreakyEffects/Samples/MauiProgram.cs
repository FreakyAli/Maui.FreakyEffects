using Microsoft.Extensions.Logging;
using Maui.FreakyEffects;
using Microsoft.Maui.Hosting;

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
            .ConfigureEffects(effects => {
                effects.InitFreakyEffects();
            });
        builder.InitSkiaSharp();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
