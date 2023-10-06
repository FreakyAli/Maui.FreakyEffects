using Maui.FreakyEffects.TouchTracking;
using SkiaSharp;

namespace Maui.FreakyEffects.SkiaScene.TouchManipulation;

public class TouchGestureRecognizer : ITouchGestureRecognizer
{
    private const int MaxTapMoveCounter = 8;

    private readonly Dictionary<long, TouchManipulationInfo> _touchDictionary =
        new Dictionary<long, TouchManipulationInfo>();
    protected DateTime LastTapTime = DateTime.MinValue;
    protected DateTime LastDoubleTapTime = DateTime.MinValue;
    protected TimeSpan DoubleTapDelay = TimeSpan.FromMilliseconds(320);
    private Timer _timer;

    public event TapEventHandler OnTap;
    public event TapEventHandler OnDoubleTap;
    public event TapEventHandler OnSingleTap;
    public event PinchEventHandler OnPinch;
    public event PanEventHandler OnPan;

    public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
    {
        switch (type)
        {
            case TouchActionType.Pressed:
                // TODO: System.ArgumentException Message = An item with the same key has already been added.Key: 1 happens after a few pinches
                var newTouchManipulation = new TouchManipulationInfo
                {
                    PreviousPoint = location,
                    NewPoint = location,
                    MoveCounter = 0
                };

                if (_touchDictionary.ContainsKey(id))
                {
                    _touchDictionary[id] = newTouchManipulation;
                }
                else
                {
                    _touchDictionary.Add(id, newTouchManipulation);
                }

                break;

            case TouchActionType.Moved:
                if (!_touchDictionary.ContainsKey(id))
                {
                    return;
                }
                TouchManipulationInfo info = _touchDictionary[id];
                info.NewPoint = location;
                info.MoveCounter = info.MoveCounter + 1;
                DetectPinchAndPanGestures(type);
                info.PreviousPoint = info.NewPoint;
                break;

            case TouchActionType.Released:
                if (!_touchDictionary.ContainsKey(id))
                {
                    return;
                }
                _touchDictionary[id].NewPoint = location;
                DetectTapGestures();
                DetectPinchAndPanGestures(type);
                _touchDictionary.Remove(id);
                break;

            case TouchActionType.Cancelled:
                if (!_touchDictionary.ContainsKey(id))
                {
                    return;
                }
                _touchDictionary.Remove(id);
                break;
        }
    }

    private void DetectTapGestures()
    {
        TouchManipulationInfo[] infos = new TouchManipulationInfo[_touchDictionary.Count];
        _touchDictionary.Values.CopyTo(infos, 0);
        if (infos.Length != 1)
        {
            return;
        }
        SKPoint point = infos[0].PreviousPoint;
        if (infos[0].MoveCounter > MaxTapMoveCounter)
        {
            return;
        }
        var tapEventArgs = new TapEventArgs(point);

        var now = DateTime.Now;
        var lastTapTime = LastTapTime;
        LastTapTime = now;

        OnTap?.Invoke(this, tapEventArgs);
        if (now - lastTapTime < DoubleTapDelay)
        {
            OnDoubleTap?.Invoke(this, tapEventArgs);
            LastDoubleTapTime = now;
            LastTapTime = DateTime.MinValue; //Reset double tap timer
        }
        else
        {
            _timer = new Timer(_ =>
            {
                if (DateTime.Now - LastDoubleTapTime < DoubleTapDelay)
                {
                    return;
                }
                OnSingleTap?.Invoke(this, tapEventArgs);
            }, null, DoubleTapDelay.Milliseconds, Timeout.Infinite);
        }
    }

    private void DetectPinchAndPanGestures(TouchActionType touchActionType)
    {
        TouchManipulationInfo[] infos = new TouchManipulationInfo[_touchDictionary.Count];
        _touchDictionary.Values.CopyTo(infos, 0);

        if (infos.Length == 1)
        {
            SKPoint previousPoint = infos[0].PreviousPoint;
            SKPoint newPoint = infos[0].NewPoint;
            OnPan?.Invoke(this, new PanEventArgs(previousPoint, newPoint, touchActionType));
        }
        else if (infos.Length >= 2)
        {
            int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
            SKPoint pivotPoint = infos[pivotIndex].NewPoint;
            SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
            SKPoint previousPoint = infos[1 - pivotIndex].PreviousPoint;
            OnPinch?.Invoke(this, new PinchEventArgs(previousPoint, newPoint, pivotPoint, touchActionType));
        }
    }
}
