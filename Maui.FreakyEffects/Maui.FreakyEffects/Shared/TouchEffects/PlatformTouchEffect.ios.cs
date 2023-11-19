#if IOS
using System.ComponentModel;
using System.Windows.Input;
using CoreFoundation;
using Foundation;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace Maui.FreakyEffects.TouchEffects;

public class CommandsPlatform : PlatformEffect
{
    public UIView View => Control ?? Container;

    DateTime _tapTime;
    ICommand _tapCommand;
    ICommand _longCommand;
    object _tapParameter;
    object _longParameter;

    protected override void OnAttached()
    {
        View.UserInteractionEnabled = true;

        UpdateTap();
        UpdateTapParameter();
        UpdateLongTap();
        UpdateLongTapParameter();

        TouchGestureCollector.Add(View, OnTouch);
    }

    protected override void OnDetached()
    {
        TouchGestureCollector.Delete(View, OnTouch);
    }

    void OnTouch(TouchGestureRecognizer.TouchArgs e)
    {
        switch (e.State)
        {
            case TouchGestureRecognizer.TouchState.Started:
                _tapTime = DateTime.Now;
                break;

            case TouchGestureRecognizer.TouchState.Ended:
                if (e.Inside)
                {
                    var range = (DateTime.Now - _tapTime).TotalMilliseconds;
                    if (range > 800)
                        LongClickHandler();
                    else
                        ClickHandler();
                }
                break;

            case TouchGestureRecognizer.TouchState.Cancelled:
                break;
        }
    }

    void ClickHandler()
    {
        if (_tapCommand?.CanExecute(_tapParameter) ?? false)
            _tapCommand.Execute(_tapParameter);
    }

    void LongClickHandler()
    {
        if (_longCommand == null)
            ClickHandler();
        else if (_longCommand.CanExecute(_longParameter))
            _longCommand.Execute(_longParameter);
    }

    protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if (args.PropertyName == Commands.TapProperty.PropertyName)
            UpdateTap();
        else if (args.PropertyName == Commands.TapParameterProperty.PropertyName)
            UpdateTapParameter();
        else if (args.PropertyName == Commands.LongTapProperty.PropertyName)
            UpdateLongTap();
        else if (args.PropertyName == Commands.LongTapParameterProperty.PropertyName)
            UpdateLongTapParameter();
    }

    void UpdateTap()
    {
        _tapCommand = Commands.GetTap(Element);
    }

    void UpdateTapParameter()
    {
        _tapParameter = Commands.GetTapParameter(Element);
    }

    void UpdateLongTap()
    {
        _longCommand = Commands.GetLongTap(Element);
    }

    void UpdateLongTapParameter()
    {
        _longParameter = Commands.GetLongTapParameter(Element);
    }

    public static void Init()
    {
    }
}

public class TouchEffectPlatform : PlatformEffect
{
    public bool IsDisposed => Container == null || Container.Handle == NativeHandle.Zero;
    public UIView View => Control ?? Container;

    UIView _layer;
    float _alpha;

    protected override void OnAttached()
    {
        View.UserInteractionEnabled = true;
        _layer = new UIView
        {
            UserInteractionEnabled = false,
            Opaque = false,
            Alpha = 0,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        UpdateEffectColor();
        TouchGestureCollector.Add(View, OnTouch);

        View.AddSubview(_layer);
        View.BringSubviewToFront(_layer);
        _layer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
        _layer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
        _layer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
        _layer.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
    }

    protected override void OnDetached()
    {
        TouchGestureCollector.Delete(View, OnTouch);
        _layer?.RemoveFromSuperview();
        _layer?.Dispose();
    }

    void OnTouch(TouchGestureRecognizer.TouchArgs e)
    {
        switch (e.State)
        {
            case TouchGestureRecognizer.TouchState.Started:
                BringLayer();
                break;

            case TouchGestureRecognizer.TouchState.Ended:
                EndAnimation();
                break;

            case TouchGestureRecognizer.TouchState.Cancelled:
                if (!IsDisposed && _layer != null)
                {
                    _layer.Layer.RemoveAllAnimations();
                    _layer.Alpha = 0;
                }

                break;
        }
    }

    protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnElementPropertyChanged(e);

        if (e.PropertyName == TouchEffect.ColorProperty.PropertyName)
        {
            UpdateEffectColor();
        }
    }

    void UpdateEffectColor()
    {
        var color = TouchEffect.GetColor(Element);
        if (color == Colors.Transparent)
        {
            return;
        }

        _alpha = color.Alpha < 1.0 ? 1 : (float)0.3;
        _layer.BackgroundColor = color.ToPlatform();
    }

    void BringLayer()
    {
        _layer.Layer.RemoveAllAnimations();
        _layer.Alpha = _alpha;
        View.BringSubviewToFront(_layer);
    }

    void EndAnimation()
    {
        if (!IsDisposed && _layer != null)
        {
            _layer.Layer.RemoveAllAnimations();
            UIView.Animate(0.225,
            () =>
            {
                _layer.Alpha = 0;
            });
        }
    }

    public static void Init()
    {
    }
}

internal static class TouchGestureCollector
{
    static Dictionary<UIView, GestureActionsContainer> Collection { get; } =
        new Dictionary<UIView, GestureActionsContainer>();

    public static void Add(UIView view, Action<TouchGestureRecognizer.TouchArgs> action)
    {
        if (Collection.ContainsKey(view))
        {
            Collection[view].Actions.Add(action);
        }
        else
        {
            var gest = new TouchGestureRecognizer
            {
                CancelsTouchesInView = false,
                Delegate = new TouchGestureRecognizerDelegate(view)
            };
            gest.OnTouch += ActionActivator;
            Collection.Add(view,
                new GestureActionsContainer
                {
                    Recognizer = gest,
                    Actions = new List<Action<TouchGestureRecognizer.TouchArgs>> { action }
                });
            view.AddGestureRecognizer(gest);
        }
    }

    public static void Delete(UIView view, Action<TouchGestureRecognizer.TouchArgs> action)
    {
        if (!Collection.ContainsKey(view)) return;

        var ci = Collection[view];
        ci.Actions.Remove(action);

        if (ci.Actions.Count != 0) return;
        view.RemoveGestureRecognizer(ci.Recognizer);
        Collection.Remove(view);
    }

    static void ActionActivator(object sender, TouchGestureRecognizer.TouchArgs e)
    {
        var gest = (TouchGestureRecognizer)sender;
        if (!Collection.ContainsKey(gest.View)) return;

        var actions = Collection[gest.View].Actions.ToArray();
        foreach (var valueAction in actions)
        {
            valueAction?.Invoke(e);
        }
    }

    class GestureActionsContainer
    {
        public TouchGestureRecognizer Recognizer { get; set; }
        public List<Action<TouchGestureRecognizer.TouchArgs>> Actions { get; set; }
    }
}

public class TouchGestureRecognizer : UIGestureRecognizer
{
    public class TouchArgs : EventArgs
    {
        public TouchState State { get; }
        public bool Inside { get; }

        public TouchArgs(TouchState state, bool inside)
        {
            State = state;
            Inside = inside;
        }
    }

    public enum TouchState
    {
        Started,
        Ended,
        Cancelled
    }

    bool _disposed;
    bool _startCalled;

    public static bool IsActive { get; private set; }

    public bool Processing => State == UIGestureRecognizerState.Began || State == UIGestureRecognizerState.Changed;
    public event EventHandler<TouchArgs> OnTouch;

    public override async void TouchesBegan(NSSet touches, UIEvent evt)
    {
        base.TouchesBegan(touches, evt);
        if (Processing)
            return;

        State = UIGestureRecognizerState.Began;
        IsActive = true;
        _startCalled = false;

        await Task.Delay(125);
        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
            if (!Processing || _disposed) return;
            OnTouch?.Invoke(this, new TouchArgs(TouchState.Started, true));
            _startCalled = true;
        });
    }

    public override void TouchesMoved(NSSet touches, UIEvent evt)
    {
        base.TouchesMoved(touches, evt);

        var inside = View.PointInside(LocationInView(View), evt);

        if (!inside)
        {
            if (_startCalled)
                OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, false));
            State = UIGestureRecognizerState.Ended;
            IsActive = false;
            return;
        }

        State = UIGestureRecognizerState.Changed;
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
        base.TouchesEnded(touches, evt);

        if (!_startCalled)
            OnTouch?.Invoke(this, new TouchArgs(TouchState.Started, true));

        OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, View.PointInside(LocationInView(View), null)));
        State = UIGestureRecognizerState.Ended;
        IsActive = false;
    }

    public override void TouchesCancelled(NSSet touches, UIEvent evt)
    {
        base.TouchesCancelled(touches, evt);
        OnTouch?.Invoke(this, new TouchArgs(TouchState.Cancelled, false));
        State = UIGestureRecognizerState.Cancelled;
        IsActive = false;
    }

    internal void TryEndOrFail()
    {
        if (_startCalled)
        {
            OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, false));
            State = UIGestureRecognizerState.Ended;
        }

        State = UIGestureRecognizerState.Failed;
        IsActive = false;
    }

    protected override void Dispose(bool disposing)
    {
        _disposed = true;
        IsActive = false;

        base.Dispose(disposing);
    }
}

public class TouchGestureRecognizerDelegate : UIGestureRecognizerDelegate
{
    readonly UIView _view;

    public TouchGestureRecognizerDelegate(UIView view)
    {
        _view = view;
    }

    public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer,
        UIGestureRecognizer otherGestureRecognizer)
    {
        if (gestureRecognizer is TouchGestureRecognizer rec && otherGestureRecognizer is UIPanGestureRecognizer &&
            otherGestureRecognizer.State == UIGestureRecognizerState.Began)
        {
            rec.TryEndOrFail();
        }

        return true;
    }

    public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
    {
        if (recognizer is TouchGestureRecognizer && TouchGestureRecognizer.IsActive)
        {
            return false;
        }

        return touch.View.IsDescendantOfView(_view);
    }
}

#endif