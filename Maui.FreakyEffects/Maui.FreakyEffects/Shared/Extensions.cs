using Maui.FreakyEffects.TouchTracking;
using Maui.FreakyEffects.TouchEffects;
using TouchEffect = Maui.FreakyEffects.TouchTracking.TouchEffect;
#if ANDROID
using PlatformTouchEffects = Maui.FreakyEffects.Platforms.Android.TouchEffect;
#elif IOS
using PlatformTouchEffects = Maui.FreakyEffects.Platforms.iOS.TouchEffect;
#endif

namespace Maui.FreakyEffects;

public static class Extensions
{
    public static void InitFreakyEffects(this IEffectsBuilder effects)
    {
        effects.Add<TouchEffect, PlatformTouchEffects>();
        effects.Add<TouchRoutingEffect, TouchEffectPlatform>();
        effects.Add<CommandsRoutingEffect, CommandsPlatform>();
    }
}