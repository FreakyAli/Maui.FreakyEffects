# Getting Started

## Requirements

- .NET 10.0+
- .NET MAUI workload installed

## Supported Platforms

| Platform | Minimum Version |
| --- | --- |
| iOS | 15.0 |
| Android | API 23 (Marshmallow) |
| macOS Catalyst | 15.0 |
| Windows | 10.0.17763 (1809) |

## Installation

Install the NuGet package into your .NET MAUI project:

```
Install-Package FreakyEffects
```

Or via the .NET CLI:

```
dotnet add package FreakyEffects
```

## Registration

Call `InitFreakyEffects()` inside `ConfigureEffects` in your `MauiProgram.cs`:

```csharp
using Maui.FreakyEffects;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureEffects(effects =>
            {
                effects.InitFreakyEffects();
            });

        return builder.Build();
    }
}
```

This registers all three effect routing pairs — `TouchRoutingEffect`, `TouchEffect` (touch tracking), and `CommandsRoutingEffect` — for every platform.

## Namespaces

Add the following XAML namespace to pages where you use the effects:

```xml
xmlns:fe="clr-namespace:Maui.FreakyEffects;assembly=Maui.FreakyEffects"
xmlns:skeleton="clr-namespace:Maui.FreakyEffects.Skeleton;assembly=Maui.FreakyEffects"
xmlns:touch="clr-namespace:Maui.FreakyEffects.TouchEffects;assembly=Maui.FreakyEffects"
xmlns:tracking="clr-namespace:Maui.FreakyEffects.TouchTracking;assembly=Maui.FreakyEffects"
```

## Next Steps

- [Skeleton Effect](skeleton-effect.md) — animated loading placeholders
- [Touch Effects](touch-effects.md) — ripple/highlight feedback and command bindings
- [Touch Tracking](touch-tracking.md) — low-level multi-touch event tracking
- [SkiaScene](skia-scene.md) — gesture-driven 2D canvas
