#if WINDOWS
using Maui.FreakyEffects.TouchTracking;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Maui.FreakyEffects.Platforms.Windows;

public class TouchHandler : TouchHandlerBase<FrameworkElement>
{
    private FrameworkElement _element;

    private static Dictionary<FrameworkElement, TouchHandler> _viewDictionary = new();
    private static Dictionary<uint, TouchHandler> _idToHandlerDictionary = new();

    public override void RegisterEvents(FrameworkElement element)
    {
        _element = element;
        _viewDictionary[element] = this;

        element.PointerPressed += OnPointerPressed;
        element.PointerMoved += OnPointerMoved;
        element.PointerReleased += OnPointerReleased;
        element.PointerCanceled += OnPointerCanceled;
        element.PointerCaptureLost += OnPointerCaptureLost;
    }

    public override void UnregisterEvents(FrameworkElement element)
    {
        _viewDictionary.Remove(element);

        element.PointerPressed -= OnPointerPressed;
        element.PointerMoved -= OnPointerMoved;
        element.PointerReleased -= OnPointerReleased;
        element.PointerCanceled -= OnPointerCanceled;
        element.PointerCaptureLost -= OnPointerCaptureLost;
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var point = e.GetCurrentPoint(element);
        uint id = point.PointerId;
        var location = new TouchTrackingPoint((float)point.Position.X, (float)point.Position.Y);

        FireEvent(this, id, TouchActionType.Pressed, location, true);

        if (!_idToHandlerDictionary.ContainsKey(id))
            _idToHandlerDictionary[id] = this;

        if (Capture)
            element.CapturePointer(e.Pointer);
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        uint id = e.GetCurrentPoint(element).PointerId;

        if (Capture)
        {
            var point = e.GetCurrentPoint(element);
            var location = new TouchTrackingPoint((float)point.Position.X, (float)point.Position.Y);
            FireEvent(this, id, TouchActionType.Moved, location, true);
        }
        else
        {
            CheckForBoundaryHop(e, id);

            if (_idToHandlerDictionary.TryGetValue(id, out var handler) && handler != null)
            {
                var point = e.GetCurrentPoint(handler._element);
                var location = new TouchTrackingPoint((float)point.Position.X, (float)point.Position.Y);
                FireEvent(handler, id, TouchActionType.Moved, location, true);
            }
        }
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        uint id = e.GetCurrentPoint(element).PointerId;

        if (Capture)
        {
            var point = e.GetCurrentPoint(element);
            var location = new TouchTrackingPoint((float)point.Position.X, (float)point.Position.Y);
            FireEvent(this, id, TouchActionType.Released, location, false);
        }
        else
        {
            CheckForBoundaryHop(e, id);

            if (_idToHandlerDictionary.TryGetValue(id, out var handler) && handler != null)
            {
                var point = e.GetCurrentPoint(handler._element);
                var location = new TouchTrackingPoint((float)point.Position.X, (float)point.Position.Y);
                FireEvent(handler, id, TouchActionType.Released, location, false);
            }
        }

        _idToHandlerDictionary.Remove(id);
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs e) =>
        HandleCancelledOrLost(e);

    private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e) =>
        HandleCancelledOrLost(e);

    private void HandleCancelledOrLost(PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(_element);
        uint id = point.PointerId;
        var location = new TouchTrackingPoint((float)point.Position.X, (float)point.Position.Y);

        if (_idToHandlerDictionary.TryGetValue(id, out var handler) && handler != null)
            FireEvent(handler, id, TouchActionType.Cancelled, location, false);

        _idToHandlerDictionary.Remove(id);
    }

    private void CheckForBoundaryHop(PointerRoutedEventArgs e, uint id)
    {
        TouchHandler hitHandler = null;

        foreach (var kvp in _viewDictionary)
        {
            var view = kvp.Key;
            var point = e.GetCurrentPoint(view);
            var bounds = new global::Windows.Foundation.Rect(0, 0, view.ActualWidth, view.ActualHeight);

            if (bounds.Contains(point.Position))
                hitHandler = kvp.Value;
        }

        _idToHandlerDictionary.TryGetValue(id, out var currentHandler);

        if (hitHandler == currentHandler)
            return;

        if (currentHandler != null)
        {
            var exitPoint = e.GetCurrentPoint(currentHandler._element);
            FireEvent(currentHandler, id, TouchActionType.Exited,
                new TouchTrackingPoint((float)exitPoint.Position.X, (float)exitPoint.Position.Y), true);
        }

        if (hitHandler != null)
        {
            var enterPoint = e.GetCurrentPoint(hitHandler._element);
            FireEvent(hitHandler, id, TouchActionType.Entered,
                new TouchTrackingPoint((float)enterPoint.Position.X, (float)enterPoint.Position.Y), true);
        }

        _idToHandlerDictionary[id] = hitHandler;
    }

    private void FireEvent(TouchHandler handler, uint id, TouchActionType actionType,
        TouchTrackingPoint location, bool isInContact)
    {
        OnTouchAction(handler._element,
            new TouchActionEventArgs(id, actionType, location, isInContact));
    }
}
#endif
