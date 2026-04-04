# Touch Effects

Touch Effects provide two things:

1. **Visual feedback** — a configurable colour overlay/ripple that appears when a user touches a view.
2. **Command bindings** — tap and long-tap `ICommand` bindings attachable to any view via XAML.

## Platform Support

| Platform | Visual Effect | Commands |
| --- | --- | --- |
| Android | Ripple (API 21+), colour overlay fallback | Yes |
| iOS | Animated UIView layer fade | Yes |
| macOS Catalyst | Animated UIView layer fade | Yes |
| Windows | Rectangle overlay with `DoubleAnimation` fade | Yes |

## XAML Namespace

```xml
xmlns:touch="clr-namespace:Maui.FreakyEffects.TouchEffects;assembly=Maui.FreakyEffects"
```

---

## TouchEffect — Visual Feedback

### Attached Property

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `TouchEffect.Color` | `Color` | `Transparent` | The highlight colour shown on press. Setting this automatically attaches the routing effect to the view. Setting it back to `Transparent` removes it |

### Usage

```xml
<Frame
    touch:TouchEffect.Color="#33000000"
    CornerRadius="12"
    Padding="16">
    <Label Text="Tap me" />
</Frame>
```

The overlay alpha is automatically reduced (to ~30%) for opaque colours so the highlight is subtle. If you provide a colour with an explicit alpha it is used as-is.

### Behaviour Notes

- The effect requires the view to be inside a container (`ContentView`, `Grid`, `StackLayout`, etc.). A `Label` or `Button` on its own will throw at runtime — wrap it.
- On Android API 21+ a native `RippleDrawable` is used. On earlier APIs a plain colour overlay fades in/out with `ObjectAnimator`.
- On iOS/macOS the overlay is a `UIView` subview constrained to fill the parent, animated with `UIView.Animate`.
- On Windows the overlay is a `Rectangle` added to the container `Panel`, animated with a WinUI `Storyboard`/`DoubleAnimation`.

---

## Commands — Tap / Long-Tap Bindings

### Attached Properties

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `Commands.Tap` | `ICommand` | `null` | Command executed on a single tap |
| `Commands.TapParameter` | `object` | `null` | Parameter passed to `Commands.Tap` |
| `Commands.LongTap` | `ICommand` | `null` | Command executed when the press exceeds 800 ms |
| `Commands.LongTapParameter` | `object` | `null` | Parameter passed to `Commands.LongTap` |

Setting either `Tap` or `LongTap` automatically attaches the routing effect. Clearing both removes it.

### Usage

```xml
<Frame
    touch:Commands.Tap="{Binding OpenDetailCommand}"
    touch:Commands.TapParameter="{Binding Item}"
    touch:Commands.LongTap="{Binding ShowContextMenuCommand}"
    touch:Commands.LongTapParameter="{Binding Item}">
    <Label Text="{Binding Item.Name}" />
</Frame>
```

### Combining with TouchEffect

Both effects can be applied to the same view simultaneously:

```xml
<Frame
    touch:TouchEffect.Color="#2200AAFF"
    touch:Commands.Tap="{Binding SelectCommand}"
    touch:Commands.TapParameter="{Binding .}">
    <Label Text="Tap with ripple" />
</Frame>
```

---

## EffectsConfig

`EffectsConfig` contains global options applied to all touch effects:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `EffectsConfig.AutoChildrenInputTransparent` | `bool` | `false` | When `true`, automatically sets `InputTransparent="True"` on child views inside a layout that has a touch effect, so touches pass through to the container |

```csharp
EffectsConfig.AutoChildrenInputTransparent = true;
```

---

## Code-Behind Usage

```csharp
// Visual effect
TouchEffect.SetColor(myView, Color.FromArgb("#33FF0000"));

// Commands
Commands.SetTap(myView, new Command(() => Console.WriteLine("Tapped")));
Commands.SetLongTap(myView, new Command(() => Console.WriteLine("Long-tapped")));
```
