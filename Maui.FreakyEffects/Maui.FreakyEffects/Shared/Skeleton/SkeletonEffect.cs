using System.ComponentModel;

namespace Maui.FreakyEffects.Skeleton;

public static class SkeletonEffect
{
    #region Public Properties

    public static readonly BindableProperty IsParentProperty = BindableProperty.CreateAttached("IsParent", typeof(bool), typeof(VisualElement), false);

    public static void SetIsParent(BindableObject b, bool value) => b.SetValue(IsParentProperty, value);

    public static bool GetIsParent(BindableObject b) => (bool)b.GetValue(IsParentProperty);

    public static readonly BindableProperty IsBusyProperty = BindableProperty.CreateAttached("IsBusy", typeof(bool), typeof(VisualElement), default(bool), propertyChanged: (b, oldValue, newValue) => OnIsBusyChanged(b, (bool)newValue));


    public static void SetIsBusy(BindableObject b, bool value) => b.SetValue(IsBusyProperty, value);

    public static bool GetIsBusy(BindableObject b) => (bool)b.GetValue(IsBusyProperty);

    public static readonly BindableProperty HideProperty = BindableProperty.CreateAttached("Hide", typeof(bool), typeof(VisualElement), default(bool));

    public static void SetHide(BindableObject b, bool value) => b.SetValue(HideProperty, value);

    public static bool GetHide(BindableObject b) => (bool)b.GetValue(HideProperty);

    public static readonly BindableProperty BackgroundColorProperty = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(VisualElement), default(Color), propertyChanged: (b, oldValue, newValue) => OnIsBusyChanged(b, GetIsBusy(b)));

    public static void SetBackgroundColor(BindableObject b, Color value) => b.SetValue(BackgroundColorProperty, value);

    public static Color GetBackgroundColor(BindableObject b) => (Color)b.GetValue(BackgroundColorProperty);

    public static readonly BindableProperty AnimationProperty = BindableProperty.CreateAttached("Animation", typeof(BaseAnimation), typeof(VisualElement), null, propertyChanged: (b, oldValue, newValue) => OnIsBusyChanged(b, GetIsBusy(b)));

    public static void SetAnimation(BindableObject b, BaseAnimation value) => b.SetValue(AnimationProperty, value);

    public static BaseAnimation GetAnimation(BindableObject b) => (BaseAnimation)b.GetValue(AnimationProperty);

    #endregion Public Properties

    #region Internal Properties

    internal static readonly BindableProperty AnimatingProperty = BindableProperty.CreateAttached("Animating", typeof(bool), typeof(VisualElement), default(bool));

    [TypeConverter(typeof(BaseAnimationTypeConverter))]
    internal static void SetAnimating(BindableObject b, bool value) => b.SetValue(AnimatingProperty, value);

    [TypeConverter(typeof(BaseAnimationTypeConverter))]
    internal static bool GetAnimating(BindableObject b) => (bool)b.GetValue(AnimatingProperty);

    internal static readonly BindableProperty CancelAnimationProperty = BindableProperty.CreateAttached("CancelAnimation", typeof(bool), typeof(VisualElement), default(bool));

    internal static void SetCancelAnimation(BindableObject b, bool value) => b.SetValue(CancelAnimationProperty, value);

    internal static bool GetCancelAnimation(BindableObject b) => (bool)b.GetValue(CancelAnimationProperty);

    internal static readonly BindableProperty OriginalBackgroundColorProperty = BindableProperty.CreateAttached("OriginalBackgroundColor", typeof(Color), typeof(VisualElement), default(Color));

    internal static void SetOriginalBackgroundColor(BindableObject b, Color value)
    {
        if (value is not null)
        {
            b.SetValue(OriginalBackgroundColorProperty, value);
        }
        else
        {
            b.ClearValue(OriginalBackgroundColorProperty);
        }
    }


    internal static Color GetOriginalBackgroundColor(BindableObject b) => (Color)b.GetValue(OriginalBackgroundColorProperty);

    internal static readonly BindableProperty UseDynamicTextColorProperty = BindableProperty.CreateAttached("UseDynamicTextColor", typeof(bool), typeof(VisualElement), default(bool));

    internal static void SetUseDynamicTextColor(BindableObject b, bool value) => b.SetValue(UseDynamicTextColorProperty, value);

    internal static bool GetUseDynamicTextColor(BindableObject b) => (bool)b.GetValue(UseDynamicTextColorProperty);

    internal static readonly BindableProperty UseDynamicBackgroundColorProperty = BindableProperty.CreateAttached("UseDynamicBackground", typeof(bool), typeof(VisualElement), default(bool));

    internal static bool GetUseDynamicBackgroundColor(BindableObject b) => (bool)b.GetValue(UseDynamicBackgroundColorProperty);

    internal static void SetUseDynamicBackgroundColor(BindableObject b, bool value) => b.SetValue(UseDynamicBackgroundColorProperty, value);

    internal static readonly BindableProperty OriginalTextColorProperty = BindableProperty.CreateAttached("OriginalTextColor", typeof(Color), typeof(VisualElement), default(Color));

    internal static void SetOriginalTextColor(BindableObject b, Color value)
    {
        if (value is not null)
        {
            b.SetValue(OriginalTextColorProperty, value);
        }
        else
        {
            b.ClearValue(OriginalTextColorProperty);
        }
    }

    internal static Color GetOriginalTextColor(BindableObject b) => (Color)b.GetValue(OriginalTextColorProperty);

    #endregion Internal Properties

    #region Operations

    private static void OnIsBusyChanged(BindableObject bindable, bool newValue)
    {
        switch (bindable)
        {
            case View view:
                HandleIsBusyChanged(view, newValue);
                break;
            case Element element:
                HandleIsBusyChanged(element, newValue);
                break;
            default:
                throw new NotSupportedException();
        }
    }
    private static void HandleIsBusyChanged(Element element, bool isBusyNewValue)
    {
        if (isBusyNewValue)
        {
            if (element is FontImageSource fis)
            {
                SetColor(fis);
            }
        }
        else
        {
            if (element is FontImageSource fis)
            {
                RestoreColor(fis);
            }
        }
    }
    private static void HandleIsBusyChanged(View view, bool isBusyNewValue)
    {
        if (isBusyNewValue)
        {
            if (GetHide(view))
            {
                ((View)view).IsVisible = false;
            }
            else
            {
                if (view is Layout layout && !GetIsParent(view))
                {
                    SetLayoutChilds(layout);
                }
                else if (view is Label || view is Button)
                {
                    SetTextColor(view);
                }

                SetBackgroundColor(view);

                RunAnimation(view);
            }
        }
        else
        {
            if (GetHide(view))
            {
                ((View)view).IsVisible = true;
            }
            else
            {
                CancelAnimation(view);

                RestoreBackgroundColor(view);

                if (view is Layout layout && !GetIsParent(view))
                {
                    RestoreLayoutChilds(layout);
                }
                else if (view is Label || view is Button)
                {
                    RestoreTextColor(view);
                }
            }
        }
    }

    private static void SetLayoutChilds(Layout layout)
    {
        if (layout.Children != null && layout.Children.Count > 0)
        {
            layout.Children.ToList().ForEach(x => ((View)x).SetValue(VisualElement.OpacityProperty, 0));
        }
    }

    private static void RestoreLayoutChilds(Layout layout)
    {
        if (layout.Children != null && layout.Children.Count > 0)
        {
            layout.Children.ToList().ForEach(x => ((View)x).SetValue(VisualElement.OpacityProperty, 1));
        }
    }

    private static void SetBackgroundColor(View view)
    {
        var hasDynamic = GetUseDynamicBackgroundColor(view)
            || view.HasDynamicColorOnProperty(VisualElement.BackgroundColorProperty);

        Color originalColor = null;
        if (view.Background is SolidColorBrush color)
        {
            originalColor = color.Color;
        }
        else if (view.Background is not null)
        {
            originalColor = view.BackgroundColor;
        }

        SetOriginalBackgroundColor(view, originalColor);
        var backgroundColor = GetBackgroundColor(view);
        if (backgroundColor != default(Color))
        {
            view.Background =
            view.BackgroundColor = backgroundColor;
        }

        SetUseDynamicBackgroundColor(view, hasDynamic);
    }

    private static void RestoreBackgroundColor(View view)
    {
        var useDynamic = GetUseDynamicBackgroundColor(view);
        if (useDynamic)
        {
            var key = view.GetPropertyDynamicResourceKey(VisualElement.BackgroundColorProperty);
            view.SetDynamicResource(VisualElement.BackgroundColorProperty, key);
        }
        else
        {
            if (GetOriginalBackgroundColor(view) is { } originalColor)
            {
                view.Background =
                view.BackgroundColor = originalColor;
            }
            else
            {
                view.ClearValue(VisualElement.BackgroundColorProperty);
            }
        }
    }

    private static void SetColor(FontImageSource view)
    {
        Color originalColor = view.Color;
        SetOriginalBackgroundColor(view, originalColor);
        var backgroundColor = GetBackgroundColor(view);
        if (backgroundColor != default(Color))
        {
            view.Color = backgroundColor;
        }
        SetUseDynamicBackgroundColor(view, false);
    }

    private static void RestoreColor(FontImageSource view)
    {
        if (GetOriginalBackgroundColor(view) is { } originalColor)
        {
            view.Color = originalColor;
        }
        else
        {
            view.ClearValue(FontImageSource.ColorProperty);
        }
    }

    private static void SetTextColor(View view)
    {
        var hasDynamic = GetUseDynamicTextColor(view);
        if (view is Label label)
        {
            hasDynamic = hasDynamic || label.HasDynamicColorOnProperty(Label.TextColorProperty);
            SetOriginalTextColor(label, label.TextColor);
            label.TextColor = Colors.Transparent;
        }
        else if (view is Button button)
        {
            hasDynamic = hasDynamic || button.HasDynamicColorOnProperty(Button.TextColorProperty);
            SetOriginalTextColor(button, button.TextColor);
            button.TextColor = Colors.Transparent;
        }

        SetUseDynamicTextColor(view, hasDynamic);
    }

    private static void RestoreTextColor(View view)
    {
        var useDynamic = GetUseDynamicTextColor(view);
        if (view is Label label)
        {
            if (useDynamic)
            {
                var key = label.GetPropertyDynamicResourceKey(Label.TextColorProperty);
                label.SetDynamicResource(Label.TextColorProperty, key);
            }
            else if (GetOriginalTextColor(view) is { } originalTextColor)
            {
                label.TextColor = originalTextColor;
            }
            else
            {
                label.ClearValue(Label.TextColorProperty);
            }
        }
        else if (view is Button button)
        {
            if (useDynamic)
            {
                var key = button.GetPropertyDynamicResourceKey(Button.TextColorProperty);
                button.SetDynamicResource(Button.TextColorProperty, key);
            }
            else if (GetOriginalTextColor(view) is { } originalTextColor)
            {
                button.TextColor = originalTextColor;
            }
            else
            {
                button.ClearValue(Button.TextColorProperty);
            }
        }
    }

    private static void RunAnimation(View view)
    {
        if (GetAnimating(view))
        {
            return;
        }
        var animation = GetAnimation(view);

        if (animation == null)
        {
            animation = new FadeAnimation();
            SetAnimation(view, animation);
        }

        SetCancelAnimation(view, false);

        if (animation != null)
        {
            animation.Start(view);
        }
    }

    private static void CancelAnimation(View view)
    {
        var animation = GetAnimation(view);

        if (animation == null)
            return;

        SetCancelAnimation(view, true);

        animation.Stop(view);
    }

    #endregion Operations
}