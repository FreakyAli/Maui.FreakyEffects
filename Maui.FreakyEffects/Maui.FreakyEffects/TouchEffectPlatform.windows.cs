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
    private bool _isPressed;

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
        var color = TouchEffects.TouchEffect.GetColor(Element);
        if (color == Colors.Transparent || _overlay == null)
            return;

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
        _isPressed = true;
        _overlay.Opacity = 1;
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_isPressed) return;
        _isPressed = false;
        AnimateFadeOut();
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs e) => CancelEffect();
    private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e) => CancelEffect();
    private void OnPointerExited(object sender, PointerRoutedEventArgs e) => CancelEffect();

    private void CancelEffect()
    {
        _isPressed = false;
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

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }
}
#endif
