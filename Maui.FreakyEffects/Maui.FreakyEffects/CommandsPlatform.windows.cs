#if WINDOWS
using System.ComponentModel;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Maui.FreakyEffects.Platforms.Windows;

internal class CommandsPlatform : PlatformEffect
{
    private FrameworkElement _view;
    private DateTime _tapTime;
    private bool _isPressed;

    protected override void OnAttached()
    {
        _view = Control ?? Container;

        _view.PointerPressed += OnPointerPressed;
        _view.PointerReleased += OnPointerReleased;
        _view.PointerCanceled += OnPointerCanceled;
        _view.PointerCaptureLost += OnPointerCaptureLost;
    }

    protected override void OnDetached()
    {
        if (_view == null) return;

        _view.PointerPressed -= OnPointerPressed;
        _view.PointerReleased -= OnPointerReleased;
        _view.PointerCanceled -= OnPointerCanceled;
        _view.PointerCaptureLost -= OnPointerCaptureLost;
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _tapTime = DateTime.Now;
        _isPressed = true;
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_isPressed) return;
        _isPressed = false;

        var duration = (DateTime.Now - _tapTime).TotalMilliseconds;
        if (duration > 800)
            LongClickHandler();
        else
            ClickHandler();
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs e) => _isPressed = false;
    private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e) => _isPressed = false;

    private void ClickHandler()
    {
        var cmd = TouchEffects.Commands.GetTap(Element);
        var param = TouchEffects.Commands.GetTapParameter(Element);
        if (cmd?.CanExecute(param) ?? false)
            cmd.Execute(param);
    }

    private void LongClickHandler()
    {
        var cmd = TouchEffects.Commands.GetLongTap(Element);
        if (cmd == null)
        {
            ClickHandler();
            return;
        }
        var param = TouchEffects.Commands.GetLongTapParameter(Element);
        if (cmd.CanExecute(param))
            cmd.Execute(param);
    }
}
#endif
