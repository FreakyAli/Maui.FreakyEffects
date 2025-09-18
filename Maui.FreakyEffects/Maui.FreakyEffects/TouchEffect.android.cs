using Maui.FreakyEffects.TouchTracking;
using View = Android.Views.View;

namespace Maui.FreakyEffects.Platforms.Android;

public class TouchEffect : Microsoft.Maui.Controls.Platform.PlatformEffect
{
    private TouchHandler _touchHandler;
    private View _view;
    private Maui.FreakyEffects.TouchTracking.TouchEffect _touchEffect;

    protected override void OnAttached()
    {
        _view = Control == null ? Container : Control;

        // Get access to the TouchEffect class in the PCL
        _touchEffect =
            (Maui.FreakyEffects.TouchTracking.TouchEffect)Element.Effects.FirstOrDefault(e => e is Maui.FreakyEffects.TouchTracking.TouchEffect);

        if (_touchEffect == null)
        {
            return;
        }
        _touchHandler = new TouchHandler();
        _touchHandler.TouchAction += TouchHandlerOnTouch;
        _touchHandler.Capture = _touchEffect.Capture;
        _touchHandler.RegisterEvents(_view);
    }

    private void TouchHandlerOnTouch(object sender, TouchActionEventArgs args)
    {
        _touchEffect.OnTouchAction(sender, args);
    }

    protected override void OnDetached()
    {
        if (_touchHandler == null)
        {
            return;
        }
        _touchHandler.TouchAction -= TouchHandlerOnTouch;
        _touchHandler.UnregisterEvents(_view);
    }
}