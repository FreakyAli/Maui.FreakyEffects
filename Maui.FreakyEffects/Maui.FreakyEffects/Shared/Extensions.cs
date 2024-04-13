using Maui.FreakyEffects.TouchEffects;
using TouchEffect = Maui.FreakyEffects.TouchTracking.TouchEffect;

#if ANDROID
using PlatformTouchEffects = Maui.FreakyEffects.Platforms.Android.TouchEffect;
#elif IOS
using PlatformTouchEffects = Maui.FreakyEffects.Platforms.iOS.TouchEffect;
#elif MACCATALYST
using Maui.FreakyEffects.Platforms.MacCatalyst;
#elif WINDOWS
using Maui.FreakyEffects.Platforms.Windows;
#else
using Maui.FreakyEffects.Platforms.Dotnet;
#endif

namespace Maui.FreakyEffects;

public static class Extensions
{
    public static void InitFreakyEffects(this IEffectsBuilder effects)
    {
        effects.Add<TouchRoutingEffect, TouchEffectPlatform>();
        effects.Add<TouchEffect, PlatformTouchEffects>();
        effects.Add<CommandsRoutingEffect, CommandsPlatform>();
    }
}