namespace Maui.FreakyEffects.TouchTracking;

public delegate void TouchActionEventHandler(object sender, TouchActionEventArgs args);

public class TouchEffect : RoutingEffect
{
    public event TouchActionEventHandler TouchAction;

    public bool Capture { set; get; }

    public void OnTouchAction(object element, TouchActionEventArgs args)
    {
        TouchAction?.Invoke(element, args);
    }
}