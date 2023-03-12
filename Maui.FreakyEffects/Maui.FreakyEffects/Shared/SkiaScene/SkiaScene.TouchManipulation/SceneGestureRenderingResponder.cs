using Maui.FreakyEffects.TouchTracking;

namespace Maui.FreakyEffects.SkiaScene.TouchManipulation;

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
