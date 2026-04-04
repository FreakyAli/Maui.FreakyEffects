#if WINDOWS
using System.Collections.Concurrent;
using Maui.FreakyEffects.TouchTracking;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace Maui.FreakyEffects.Platforms.Windows;

public class TouchHandler : TouchHandlerBase<FrameworkElement>
{
    private FrameworkElement _element;

    private static readonly ConcurrentDictionary<FrameworkElement, TouchHandler> _viewDictionary = new();
    private static readonly ConcurrentDictionary<uint, TouchHandler> _idToHandlerDictionary = new();

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
        _viewDictionary.TryRemove(element, out _);

        // Remove any active pointer IDs that were tracking this handler to prevent leaks
        foreach (var key in _idToHandlerDictionary.Keys)
        {
            if (_idToHandlerDictionary.TryGetValue(key, out var handler) && handler == this)
                _idToHandlerDictionary.TryRemove(key, out _);
        }

        _element = null;

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
            if (!_idToHandlerDictionary.ContainsKey(id))
                return;
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
            if (!_idToHandlerDictionary.TryGetValue(id, out var owner) || owner != this)
            {
                _idToHandlerDictionary.TryRemove(id, out _);
                return;
            }
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

        _idToHandlerDictionary.TryRemove(id, out _);
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

        if (!_idToHandlerDictionary.TryGetValue(id, out var handler) || handler != this)
        {
            _idToHandlerDictionary.TryRemove(id, out _);
            return;
        }

        FireEvent(this, id, TouchActionType.Cancelled, location, false);
        _idToHandlerDictionary.TryRemove(id, out _);
    }

    private void CheckForBoundaryHop(PointerRoutedEventArgs e, uint id)
    {
        // Collect all tracked views that contain the pointer, then pick the topmost
        // by Z-index (highest first) with visual-tree depth as a tiebreaker.
        TouchHandler hitHandler = _viewDictionary
            .Where(kvp =>
            {
                var point = e.GetCurrentPoint(kvp.Key);
                var bounds = new global::Windows.Foundation.Rect(0, 0, kvp.Key.ActualWidth, kvp.Key.ActualHeight);
                return bounds.Contains(point.Position);
            })
            .OrderByDescending(kvp => Canvas.GetZIndex(kvp.Key))
            .ThenByDescending(kvp => GetVisualDepth(kvp.Key))
            .Select(kvp => kvp.Value)
            .FirstOrDefault();

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
        handler.OnTouchAction(handler._element,
            new TouchActionEventArgs(id, actionType, location, isInContact));
    }

    private static int GetVisualDepth(DependencyObject element)
    {
        int depth = 0;
        var current = VisualTreeHelper.GetParent(element);
        while (current != null)
        {
            depth++;
            current = VisualTreeHelper.GetParent(current);
        }
        return depth;
    }
}
#endif
