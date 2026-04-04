#if WINDOWS
using System.ComponentModel;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace Maui.FreakyEffects.Platforms.Windows;

internal class TouchEffectPlatform : PlatformEffect
{
    private FrameworkElement _view;
    private Rectangle _overlay;
    private readonly HashSet<uint> _activePointerIds = new();
    private Storyboard _fadeStoryboard;

    protected override void OnAttached()
    {
        _view = Control ?? Container;

        if (Container is not Panel panel)
            return;

        _overlay = new Rectangle
        {
            IsHitTestVisible = false,
            Opacity = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        panel.Children.Add(_overlay);

        UpdateEffectColor();

        _view.PointerPressed += OnPointerPressed;
        _view.PointerReleased += OnPointerReleased;
        _view.PointerCanceled += OnPointerCanceled;
        _view.PointerCaptureLost += OnPointerCaptureLost;
        _view.PointerExited += OnPointerExited;
    }

    protected override void OnDetached()
    {
        if (_view == null) return;

        _view.PointerPressed -= OnPointerPressed;
        _view.PointerReleased -= OnPointerReleased;
        _view.PointerCanceled -= OnPointerCanceled;
        _view.PointerCaptureLost -= OnPointerCaptureLost;
        _view.PointerExited -= OnPointerExited;

        if (Container is Panel panel && _overlay != null)
            panel.Children.Remove(_overlay);

        _overlay = null;
    }

    protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnElementPropertyChanged(e);

        if (e.PropertyName == TouchEffects.TouchEffect.ColorProperty.PropertyName)
            UpdateEffectColor();
    }

    private void UpdateEffectColor()
    {
        if (_overlay == null)
            return;

        var color = TouchEffects.TouchEffect.GetColor(Element);
        if (color == Colors.Transparent)
        {
            _overlay.Fill = null;
            return;
        }

        var alpha = color.Alpha < 1.0f ? color.Alpha : 0.3f;
        var winColor = global::Windows.UI.Color.FromArgb(
            (byte)(alpha * 255),
            (byte)(color.Red * 255),
            (byte)(color.Green * 255),
            (byte)(color.Blue * 255));

        _overlay.Fill = new SolidColorBrush(winColor);
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_overlay == null) return;
        var id = e.GetCurrentPoint(_view).PointerId;
        if (_activePointerIds.Add(id) && _activePointerIds.Count == 1)
        {
            _fadeStoryboard?.Stop();
            _fadeStoryboard = null;
            _overlay.Opacity = 1;
        }
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var id = e.GetCurrentPoint(_view).PointerId;
        _activePointerIds.Remove(id);
        if (_activePointerIds.Count == 0)
            AnimateFadeOut();
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs e) =>
        CancelPointer(e.GetCurrentPoint(_view).PointerId);

    private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e) =>
        CancelPointer(e.GetCurrentPoint(_view).PointerId);

    private void OnPointerExited(object sender, PointerRoutedEventArgs e) =>
        CancelPointer(e.GetCurrentPoint(_view).PointerId);

    private void CancelPointer(uint id)
    {
        _activePointerIds.Remove(id);
        if (_activePointerIds.Count == 0)
            CancelEffect();
    }

    private void CancelEffect()
    {
        _activePointerIds.Clear();
        _fadeStoryboard?.Stop();
        _fadeStoryboard = null;
        if (_overlay != null)
            _overlay.Opacity = 0;
    }

    private void AnimateFadeOut()
    {
        if (_overlay == null) return;

        var animation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(225)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(animation, _overlay);
        Storyboard.SetTargetProperty(animation, "Opacity");

        _fadeStoryboard?.Stop();
        _fadeStoryboard = new Storyboard();
        _fadeStoryboard.Children.Add(animation);
        _fadeStoryboard.Begin();
    }
}
#endif
