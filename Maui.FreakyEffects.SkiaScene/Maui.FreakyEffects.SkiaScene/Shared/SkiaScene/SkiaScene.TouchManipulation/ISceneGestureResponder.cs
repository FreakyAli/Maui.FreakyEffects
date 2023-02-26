using System;
using Maui.FreakyEffects.SkiaScene;
using Maui.FreakyEffects.TouchTracking;
using SkiaSharp;

namespace Maui.FreakyEffects.SkiaScene.TouchManipulation;

public interface ISceneGestureResponder
{
    TouchManipulationMode TouchManipulationMode { get; set; }
    void StartResponding();
    void StopResponding();
}

public interface ITouchGestureRecognizer
{
    void ProcessTouchEvent(long id, TouchActionType type, SKPoint location);
    event TapEventHandler OnTap;
    event TapEventHandler OnDoubleTap;
    event TapEventHandler OnSingleTap;
    event PinchEventHandler OnPinch;
    event PanEventHandler OnPan;
}

public class PanEventArgs : EventArgs
{
    public SKPoint PreviousPoint { get; }

    public SKPoint NewPoint { get; }

    public TouchActionType TouchActionType { get; }

    public PanEventArgs(SKPoint previousPoint, SKPoint newPoint, TouchActionType touchActionType)
    {
        PreviousPoint = previousPoint;
        NewPoint = newPoint;
        TouchActionType = touchActionType;
    }
}

public delegate void PanEventHandler(object sender, PanEventArgs args);

public delegate void PinchEventHandler(object sender, PinchEventArgs args);


public class PinchEventArgs : EventArgs
{
    public SKPoint PreviousPoint { get; }

    public SKPoint NewPoint { get; }

    public SKPoint PivotPoint { get; }

    public TouchActionType TouchActionType { get; }

    public PinchEventArgs(SKPoint previousPoint, SKPoint newPoint, SKPoint pivotPoint, TouchActionType touchActionType)
    {
        PreviousPoint = previousPoint;
        NewPoint = newPoint;
        PivotPoint = pivotPoint;
        TouchActionType = touchActionType;
    }
}

public class SceneGestureRenderingResponder : SceneGestureResponder
{
    private readonly Action _invalidateViewAction;

    private const int DefaultFramesPerSecond = 30;

    private TimeSpan _minGestureDuration = TimeSpan.FromMilliseconds(60);
    private DateTime _lastPanTime = DateTime.MinValue;
    private DateTime _lastPinchTime = DateTime.MinValue;
    private int _maxFramesPerSecond;

    public SceneGestureRenderingResponder(Action invalidateViewAction, ISKScene skScene,
        ITouchGestureRecognizer touchGestureRecognizer) : base(skScene, touchGestureRecognizer)
    {
        _invalidateViewAction = invalidateViewAction;
        MaxFramesPerSecond = DefaultFramesPerSecond;
    }

    public int MaxFramesPerSecond
    {
        get { return _maxFramesPerSecond; }
        set
        {
            _maxFramesPerSecond = value;
            _minGestureDuration = TimeSpan.FromMilliseconds(1000d / _maxFramesPerSecond);
        }
    }

    protected override void TouchGestureRecognizerOnPan(object sender, PanEventArgs args)
    {
        base.TouchGestureRecognizerOnPan(sender, args);

        if (args.TouchActionType == TouchActionType.Released)
        {
            _invalidateViewAction();
        }
        ProcessGestureDelays(ref _lastPanTime, _minGestureDuration, args.TouchActionType);
    }

    protected override void TouchGestureRecognizerOnPinch(object sender, PinchEventArgs args)
    {
        base.TouchGestureRecognizerOnPinch(sender, args);

        ProcessGestureDelays(ref _lastPinchTime, _minGestureDuration, args.TouchActionType);
    }

    private void ProcessGestureDelays(ref DateTime lastGestureTime, TimeSpan minGestureDuration, TouchActionType touchActionType)
    {
        if (touchActionType != TouchActionType.Moved)
        {
            lastGestureTime = DateTime.MinValue;
            _invalidateViewAction();
            return;
        }
        var now = DateTime.Now;
        if (now - lastGestureTime < minGestureDuration)
        {
            return;
        }
        lastGestureTime = now;
        _invalidateViewAction();
    }
}

public class SceneGestureResponder : ISceneGestureResponder
{
    private readonly ISKScene _skScene;
    private readonly ITouchGestureRecognizer _touchGestureRecognizer;

    public SceneGestureResponder(ISKScene skScene, ITouchGestureRecognizer touchGestureRecognizer)
    {
        _skScene = skScene;
        _touchGestureRecognizer = touchGestureRecognizer;
    }

    public TouchManipulationMode TouchManipulationMode { get; set; }
    public bool EnableTwoFingersPanInIsotropicScaleMode { get; set; }
    public float DoubleTapScaleFactor { get; set; } = 2f;

    public void StartResponding()
    {
        _touchGestureRecognizer.OnPan += TouchGestureRecognizerOnPan;
        _touchGestureRecognizer.OnPinch += TouchGestureRecognizerOnPinch;
        _touchGestureRecognizer.OnDoubleTap += TouchGestureRecognizerOnDoubleTap;
    }
    public void StopResponding()
    {
        _touchGestureRecognizer.OnPan -= TouchGestureRecognizerOnPan;
        _touchGestureRecognizer.OnPinch -= TouchGestureRecognizerOnPinch;
        _touchGestureRecognizer.OnDoubleTap -= TouchGestureRecognizerOnDoubleTap;
    }


    protected virtual void TouchGestureRecognizerOnPinch(object sender, PinchEventArgs args)
    {
        if (args.TouchActionType != TouchActionType.Moved)
            return;

        var previousPoint = args.PreviousPoint;
        var newPoint = args.NewPoint;
        var pivotPoint = args.PivotPoint;
        var transformedPivotPoint = _skScene.GetCanvasPointFromViewPoint(pivotPoint);

        SKPoint oldVector = previousPoint - pivotPoint;
        SKPoint newVector = newPoint - pivotPoint;

        if (TouchManipulationMode == TouchManipulationMode.ScaleRotate)
        {
            float angle = GetAngleBetweenVectors(oldVector, newVector);

            _skScene.RotateByRadiansDelta(transformedPivotPoint, angle);

            // Effectively rotate the old vector
            float magnitudeRatio = oldVector.GetMagnitude() / newVector.GetMagnitude();
            oldVector.X = magnitudeRatio * newVector.X;
            oldVector.Y = magnitudeRatio * newVector.Y;
        }
        else if (TouchManipulationMode == TouchManipulationMode.IsotropicScale
            && EnableTwoFingersPanInIsotropicScaleMode)
        {
            float angle = GetAngleBetweenVectors(oldVector, newVector);

            // Calculate the movement as a distance between old vector and a new vector
            // but in orthogonal direction to the old vector.

            float oldVectorOriginPoint = newVector.GetMagnitude() * (float)Math.Cos(angle);
            float magnitudeRatio = oldVectorOriginPoint / oldVector.GetMagnitude();
            SKPoint oldVectorOrigin = new SKPoint(oldVector.X * magnitudeRatio, oldVector.Y * magnitudeRatio);
            SKPoint moveVector = newVector - oldVectorOrigin;
            SKPoint dividedMoveVector = new SKPoint(moveVector.X * 0.5f, moveVector.Y * 0.5f);

            _skScene.MoveByVector(dividedMoveVector);
        }

        var scale = newVector.GetMagnitude() / oldVector.GetMagnitude();

        if (!float.IsNaN(scale) && !float.IsInfinity(scale))
        {
            _skScene.ZoomByScaleFactor(transformedPivotPoint, scale);
        }
    }

    protected virtual void TouchGestureRecognizerOnPan(object sender, PanEventArgs args)
    {
        if (args.TouchActionType != TouchActionType.Moved)
            return;

        SKPoint resultVector = args.NewPoint - args.PreviousPoint;
        _skScene.MoveByVector(resultVector);
    }

    protected virtual void TouchGestureRecognizerOnDoubleTap(object sender, TapEventArgs args)
    {
        SKPoint scenePoint = _skScene.GetCanvasPointFromViewPoint(args.ViewPoint);
        _skScene.ZoomByScaleFactor(scenePoint, DoubleTapScaleFactor);
    }

    private float GetAngleBetweenVectors(SKPoint oldVector, SKPoint newVector)
    {
        // Find angles from pivot point to touch points
        float oldAngle = (float)Math.Atan2(oldVector.Y, oldVector.X);
        float newAngle = (float)Math.Atan2(newVector.Y, newVector.X);

        float angle = newAngle - oldAngle;
        return angle;
    }
}

public class TapEventArgs : EventArgs
{
    public SKPoint ViewPoint { get; }

    public TapEventArgs(SKPoint viewPoint)
    {
        ViewPoint = viewPoint;
    }
}

public delegate void TapEventHandler(object sender, TapEventArgs args);

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
                _touchDictionary.Add(id, new TouchManipulationInfo
                {
                    PreviousPoint = location,
                    NewPoint = location,
                    MoveCounter = 0
                });
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

public class TouchManipulationInfo
{
    public SKPoint PreviousPoint { set; get; }

    public SKPoint NewPoint { set; get; }

    public int MoveCounter { get; set; }
}

public enum TouchManipulationMode
{
    IsotropicScale,
    ScaleRotate
}