#if WINDOWS
using Maui.FreakyEffects.TouchTracking;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml;

namespace Maui.FreakyEffects.Platforms.Windows;

internal class PlatformTouchEffects : PlatformEffect
{
    private TouchHandler _touchHandler;
    private FrameworkElement _element;
    private Maui.FreakyEffects.TouchTracking.TouchEffect _touchEffect;

    protected override void OnAttached()
    {
        _element = Control == null ? Container : Control;

        _touchEffect = (Maui.FreakyEffects.TouchTracking.TouchEffect)Element.Effects
            .FirstOrDefault(e => e is Maui.FreakyEffects.TouchTracking.TouchEffect);

        if (_touchEffect == null)
            return;

        _touchHandler = new TouchHandler();
        _touchHandler.TouchAction += OnTouchAction;
        _touchHandler.Capture = _touchEffect.Capture;
        _touchHandler.RegisterEvents(_element);
    }

    private void OnTouchAction(object sender, TouchActionEventArgs args)
    {
        _touchEffect.OnTouchAction(sender, args);
    }

    protected override void OnDetached()
    {
        if (_touchHandler == null)
            return;

        _touchHandler.TouchAction -= OnTouchAction;
        _touchHandler.UnregisterEvents(_element);
    }
}
#endif
