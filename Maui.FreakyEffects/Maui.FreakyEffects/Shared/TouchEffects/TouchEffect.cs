using System.Windows.Input;

namespace Maui.FreakyEffects.TouchEffects;

public static class TouchEffect
{
     public static readonly BindableProperty ColorProperty =
        BindableProperty.CreateAttached(
            "Color",
            typeof(Color),
            typeof(TouchEffect),
            Colors.Transparent,
            propertyChanged: PropertyChanged
        );

    public static void SetColor(BindableObject view, Color value)
    {
        view.SetValue(ColorProperty, value);
    }

    public static Color GetColor(BindableObject view)
    {
        return (Color)view.GetValue(ColorProperty);
    }

    static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is View view))
            return;

        var eff = view.Effects.FirstOrDefault(e => e is TouchRoutingEffect);
        if (GetColor(bindable) != Colors.Transparent)
        {
            view.InputTransparent = false;

            if (eff != null) return;
            view.Effects.Add(new TouchRoutingEffect());
            if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                !EffectsConfig.GetChildrenInputTransparent(view))
            {
                EffectsConfig.SetChildrenInputTransparent(view, true);
            }
        }
        else
        {
            if (eff == null || view.BindingContext == null) return;
            view.Effects.Remove(eff);
            if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                EffectsConfig.GetChildrenInputTransparent(view))
            {
                EffectsConfig.SetChildrenInputTransparent(view, false);
            }
        }
    }
}

public class TouchRoutingEffect : RoutingEffect
{
}

public static class EffectsConfig
{
    [Obsolete("Not need with usual Linking")]
    public static void Init()
    {
    }

    public static bool AutoChildrenInputTransparent { get; set; } = true;

    public static readonly BindableProperty ChildrenInputTransparentProperty =
        BindableProperty.CreateAttached(
            "ChildrenInputTransparent",
            typeof(bool),
            typeof(EffectsConfig),
            false,
            propertyChanged: (bindable, oldValue, newValue) => {
                ConfigureChildrenInputTransparent(bindable);
            }
        );

    public static void SetChildrenInputTransparent(BindableObject view, bool value)
    {
        view.SetValue(ChildrenInputTransparentProperty, value);
    }

    public static bool GetChildrenInputTransparent(BindableObject view)
    {
        return (bool)view.GetValue(ChildrenInputTransparentProperty);
    }

    static void ConfigureChildrenInputTransparent(BindableObject bindable)
    {
        if (!(bindable is Layout layout))
            return;

        if (GetChildrenInputTransparent(bindable))
        {
            foreach (View layoutChild in layout.Children)
                AddInputTransparentToElement(layoutChild);
            layout.ChildAdded += Layout_ChildAdded;
        }
        else
        {
            layout.ChildAdded -= Layout_ChildAdded;
        }
    }

    static void Layout_ChildAdded(object sender, ElementEventArgs e)
    {
        AddInputTransparentToElement(e.Element);
    }

    static void AddInputTransparentToElement(BindableObject obj)
    {
        if (obj is View view && TouchEffect.GetColor(view) == Colors.Transparent && Commands.GetTap(view) == null && Commands.GetLongTap(view) == null)
        {
            view.InputTransparent = true;
        }
    }
}

public static class Commands
{
    [Obsolete("Not need with usual Linking")]
    public static void Init()
    {
    }

    public static readonly BindableProperty TapProperty =
        BindableProperty.CreateAttached(
            "Tap",
            typeof(ICommand),
            typeof(Commands),
            default(ICommand),
            propertyChanged: PropertyChanged
        );

    public static void SetTap(BindableObject view, ICommand value)
    {
        view.SetValue(TapProperty, value);
    }

    public static ICommand GetTap(BindableObject view)
    {
        return (ICommand)view.GetValue(TapProperty);
    }

    public static readonly BindableProperty TapParameterProperty =
        BindableProperty.CreateAttached(
            "TapParameter",
            typeof(object),
            typeof(Commands),
            default(object),
            propertyChanged: PropertyChanged
        );

    public static void SetTapParameter(BindableObject view, object value)
    {
        view.SetValue(TapParameterProperty, value);
    }

    public static object GetTapParameter(BindableObject view)
    {
        return view.GetValue(TapParameterProperty);
    }

    public static readonly BindableProperty LongTapProperty =
        BindableProperty.CreateAttached(
            "LongTap",
            typeof(ICommand),
            typeof(Commands),
            default(ICommand),
            propertyChanged: PropertyChanged
        );

    public static void SetLongTap(BindableObject view, ICommand value)
    {
        view.SetValue(LongTapProperty, value);
    }

    public static ICommand GetLongTap(BindableObject view)
    {
        return (ICommand)view.GetValue(LongTapProperty);
    }

    public static readonly BindableProperty LongTapParameterProperty =
        BindableProperty.CreateAttached(
            "LongTapParameter",
            typeof(object),
            typeof(Commands),
            default(object)
        );

    public static void SetLongTapParameter(BindableObject view, object value)
    {
        view.SetValue(LongTapParameterProperty, value);
    }

    public static object GetLongTapParameter(BindableObject view)
    {
        return view.GetValue(LongTapParameterProperty);
    }

    static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is View view))
            return;

        var eff = view.Effects.FirstOrDefault(e => e is CommandsRoutingEffect);

        if (GetTap(bindable) != null || GetLongTap(bindable) != null)
        {
            view.InputTransparent = false;

            if (eff != null) return;
            view.Effects.Add(new CommandsRoutingEffect());
            if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                !EffectsConfig.GetChildrenInputTransparent(view))
            {
                EffectsConfig.SetChildrenInputTransparent(view, true);
            }
        }
        else
        {
            if (eff == null || view.BindingContext == null) return;
            view.Effects.Remove(eff);
            if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                EffectsConfig.GetChildrenInputTransparent(view))
            {
                EffectsConfig.SetChildrenInputTransparent(view, false);
            }
        }
    }
}

public class CommandsRoutingEffect : RoutingEffect
{
}