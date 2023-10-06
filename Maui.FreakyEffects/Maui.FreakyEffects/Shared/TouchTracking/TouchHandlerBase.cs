namespace Maui.FreakyEffects.TouchTracking;

public class TouchHandlerBase<TElement>
{
    public event TouchActionEventHandler TouchAction;

    public TouchHandlerBase()
    {
    }

    public bool Capture { set; get; } = true;

    public void OnTouchAction(TElement element, TouchActionEventArgs args)
    {
        TouchAction?.Invoke(element, args);
    }

    public virtual void RegisterEvents(TElement element) { }

    public virtual void UnregisterEvents(TElement element) { }
}

