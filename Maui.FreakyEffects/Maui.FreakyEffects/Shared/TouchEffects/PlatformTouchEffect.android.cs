#if ANDROID
using System.ComponentModel;
using Android.Animation;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Color = Android.Graphics.Color;
using ListView = Android.Widget.ListView;
using ScrollView = Android.Widget.ScrollView;
using View = Android.Views.View;
using Rect = Android.Graphics.Rect;

namespace Maui.FreakyEffects.TouchEffects;

public class TouchEffectPlatform : PlatformEffect
{
    public bool EnableRipple => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
    public bool IsDisposed => Container == null || Container.Handle == IntPtr.Zero;
    public View View => Control ?? Container;

    Color _color;
    byte _alpha;
    RippleDrawable _ripple;
    FrameLayout _viewOverlay;
    ObjectAnimator _animator;

    protected override void OnAttached()
    {
        if (Control is ListView || Control is ScrollView)
        {
            return;
        }

        if (Container is not ViewGroup group)
        {
            throw new InvalidOperationException("Touch color effect requires to be attached to a container like a ContentView or a layout (Grid, StackLayout, etc...)");
        }

        View.Clickable = true;
        View.LongClickable = true;
        _viewOverlay = new FrameLayout(Container.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
            Clickable = false,
            Focusable = false,
        };
        Container.LayoutChange += ViewOnLayoutChange;

        if (EnableRipple)
            _viewOverlay.Background = CreateRipple(_color);

        SetEffectColor();
        TouchCollector.Add(View, OnTouch);

        group.AddView(_viewOverlay);
        _viewOverlay.BringToFront();
    }

    protected override void OnDetached()
    {
        if (IsDisposed) return;

        if (Container is not ViewGroup group)
        {
            return;
        }

        group.RemoveView(_viewOverlay);
        _viewOverlay.Pressed = false;
        _viewOverlay.Foreground = null;
        _viewOverlay.Dispose();
        Container.LayoutChange -= ViewOnLayoutChange;

        if (EnableRipple)
            _ripple?.Dispose();

        TouchCollector.Delete(View, OnTouch);
    }

    protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnElementPropertyChanged(e);

        if (e.PropertyName == TouchEffect.ColorProperty.PropertyName)
        {
            SetEffectColor();
        }
    }

    void SetEffectColor()
    {
        var color = TouchEffect.GetColor(Element);
        if (color == Colors.Transparent)
        {
            return;
        }

        _color = color.ToAndroid();
        _alpha = _color.A == 255 ? (byte)80 : _color.A;

        if (EnableRipple)
        {
            _ripple.SetColor(GetPressedColorSelector(_color));
        }
    }

    void OnTouch(View.TouchEventArgs args)
    {
        switch (args.Event.Action)
        {
            case MotionEventActions.Down:
                if (EnableRipple)
                    ForceStartRipple(args.Event.GetX(), args.Event.GetY());
                else
                    BringLayer();

                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                if (IsDisposed) return;

                if (EnableRipple)
                    ForceEndRipple();
                else
                    TapAnimation(250, _alpha, 0);

                break;
        }
    }

    void ViewOnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs)
    {
        var group = (ViewGroup)sender;
        if (group == null || IsDisposed) return;
        _viewOverlay.Right = group.Width;
        _viewOverlay.Bottom = group.Height;
    }

    #region Ripple

    RippleDrawable CreateRipple(Color color)
    {
        if (Element is Layout)
        {
            var mask = new ColorDrawable(Color.White);
            return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
        }

        var back = View.Background;
        if (back == null)
        {
            var mask = new ColorDrawable(Color.White);
            return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
        }

        if (back is RippleDrawable)
        {
            _ripple = (RippleDrawable)back.GetConstantState().NewDrawable();
            _ripple.SetColor(GetPressedColorSelector(color));

            return _ripple;
        }

        return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
    }

    static ColorStateList GetPressedColorSelector(int pressedColor)
    {
        return new ColorStateList(
            new[] { new int[] { } },
            new[] { pressedColor, });
    }

    void ForceStartRipple(float x, float y)
    {
        if (IsDisposed || !(_viewOverlay.Background is RippleDrawable bc)) return;

        _viewOverlay.BringToFront();
        bc.SetHotspot(x, y);
        _viewOverlay.Pressed = true;
    }

    void ForceEndRipple()
    {
        if (IsDisposed) return;

        _viewOverlay.Pressed = false;
    }

    #endregion

    #region Overlay

    void BringLayer()
    {
        if (IsDisposed)
            return;

        ClearAnimation();

        _viewOverlay.BringToFront();
        var color = _color;
        color.A = _alpha;
        _viewOverlay.SetBackgroundColor(color);
    }

    void TapAnimation(long duration, byte startAlpha, byte endAlpha)
    {
        if (IsDisposed)
            return;

        _viewOverlay.BringToFront();

        var start = _color;
        var end = _color;
        start.A = startAlpha;
        end.A = endAlpha;

        ClearAnimation();
        _animator = ObjectAnimator.OfObject(_viewOverlay,
            "BackgroundColor",
            new ArgbEvaluator(),
            start.ToArgb(),
            end.ToArgb());
        _animator.SetDuration(duration);
        _animator.RepeatCount = 0;
        _animator.RepeatMode = ValueAnimatorRepeatMode.Restart;
        _animator.Start();
        _animator.AnimationEnd += AnimationOnAnimationEnd;
    }

    void AnimationOnAnimationEnd(object sender, EventArgs eventArgs)
    {
        if (IsDisposed) return;

        ClearAnimation();
    }

    void ClearAnimation()
    {
        if (_animator == null) return;
        _animator.AnimationEnd -= AnimationOnAnimationEnd;
        _animator.Cancel();
        _animator.Dispose();
        _animator = null;
    }

    #endregion
}

public class CommandsPlatform : PlatformEffect
{
    public View View => Control ?? Container;
    public bool IsDisposed => (Container as IVisualElementRenderer)?.Element == null;

    DateTime _tapTime;
    readonly Rect _rect = new Rect();
    readonly int[] _location = new int[2];

    public static void Init()
    {
    }

    protected override void OnAttached()
    {
        View.Clickable = true;
        View.LongClickable = true;
        View.SoundEffectsEnabled = true;
        TouchCollector.Add(View, OnTouch);
    }

    void OnTouch(View.TouchEventArgs args)
    {
        switch (args.Event.Action)
        {
            case MotionEventActions.Down:
                _tapTime = DateTime.Now;
                break;

            case MotionEventActions.Up:
                if (IsViewInBounds((int)args.Event.RawX, (int)args.Event.RawY))
                {
                    var range = (DateTime.Now - _tapTime).TotalMilliseconds;
                    if (range > 800)
                        LongClickHandler();
                    else
                        ClickHandler();
                }
                break;
        }
    }

    bool IsViewInBounds(int x, int y)
    {
        View.GetDrawingRect(_rect);
        View.GetLocationOnScreen(_location);
        _rect.Offset(_location[0], _location[1]);
        return _rect.Contains(x, y);
    }

    void ClickHandler()
    {
        var cmd = Commands.GetTap(Element);
        var param = Commands.GetTapParameter(Element);
        if (cmd?.CanExecute(param) ?? false)
            cmd.Execute(param);
    }

    void LongClickHandler()
    {
        var cmd = Commands.GetLongTap(Element);

        if (cmd == null)
        {
            ClickHandler();
            return;
        }

        var param = Commands.GetLongTapParameter(Element);
        if (cmd.CanExecute(param))
            cmd.Execute(param);
    }

    protected override void OnDetached()
    {
        if (IsDisposed) return;
        TouchCollector.Delete(View, OnTouch);
    }
}

internal static class TouchCollector
{
    static Dictionary<View, List<Action<View.TouchEventArgs>>> Collection { get; } =
        new Dictionary<View, List<Action<View.TouchEventArgs>>>();

    static View _activeView;

    public static void Add(View view, Action<View.TouchEventArgs> action)
    {
        if (Collection.ContainsKey(view))
        {
            Collection[view].Add(action);
        }
        else
        {
            view.Touch += ActionActivator;
            Collection.Add(view, new List<Action<View.TouchEventArgs>> { action });
        }
    }

    public static void Delete(View view, Action<View.TouchEventArgs> action)
    {
        if (!Collection.ContainsKey(view)) return;

        var actions = Collection[view];
        actions.Remove(action);

        if (actions.Count != 0) return;
        view.Touch -= ActionActivator;
        Collection.Remove(view);
    }

    static void ActionActivator(object sender, View.TouchEventArgs e)
    {
        var view = (View)sender;
        if (!Collection.ContainsKey(view) || (_activeView != null && _activeView != view)) return;

        switch (e.Event.Action)
        {
            case MotionEventActions.Down:
                _activeView = view;
                view.PlaySoundEffect(SoundEffects.Click);
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                _activeView = null;
                e.Handled = true;
                break;
        }

        var actions = Collection[view].ToArray();
        foreach (var valueAction in actions)
        {
            valueAction?.Invoke(e);
        }
    }
}
#endif