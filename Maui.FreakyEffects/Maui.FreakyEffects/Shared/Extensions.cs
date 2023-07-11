using Maui.FreakyEffects.TouchPress;
using Maui.FreakyEffects.TouchTracking;

#if ANDROID
using PlatformTouchAndPressEffect = Maui.FreakyEffects.Platforms.Android.TouchAndPressEffect;
using PlatformTouchEffects = Maui.FreakyEffects.Platforms.Android.TouchEffect;
using PlatformTouchReleaseEffect = Maui.FreakyEffects.Platforms.Android.TouchReleaseEffect;

#elif IOS
using PlatformTouchAndPressEffect = Maui.FreakyEffects.Platforms.iOS.TouchAndPressEffect;
using PlatformTouchEffects = Maui.FreakyEffects.Platforms.iOS.TouchEffect;
using PlatformTouchReleaseEffect = Maui.FreakyEffects.Platforms.iOS.TouchReleaseEffect;
#endif

namespace Maui.FreakyEffects;

public static class Extensions
{
    public static void InitFreakyEffects(this IEffectsBuilder effects)
    {
        effects.Add<TouchEffect, PlatformTouchEffects>();
        effects.Add<TouchAndPressEffect, PlatformTouchAndPressEffect>();
        effects.Add<TouchReleaseEffect, PlatformTouchReleaseEffect>();
    }
}