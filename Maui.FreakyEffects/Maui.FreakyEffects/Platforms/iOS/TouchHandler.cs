using Maui.FreakyEffects.TouchTracking;
using UIKit;

namespace Maui.FreakyEffects.Platforms.iOS;

public class TouchHandler : TouchHandlerBase<UIView>
{
    TouchRecognizer _touchRecognizer;

    public override void RegisterEvents(UIView view)
    {
        if (view != null)
        {
            _touchRecognizer = new TouchRecognizer(view, this);
            view.AddGestureRecognizer(_touchRecognizer);
        }
    }

    public override void UnregisterEvents(UIView view)
    {
        if (_touchRecognizer != null)
        {
            // Clean up the TouchRecognizer object
            _touchRecognizer.Detach();

            // Remove the TouchRecognizer from the UIView
            view.RemoveGestureRecognizer(_touchRecognizer);
        }
    }
}