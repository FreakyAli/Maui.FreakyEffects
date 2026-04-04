# Touch Tracking

Touch Tracking is a low-level, cross-platform multi-touch event system. It fires granular events — `Pressed`, `Moved`, `Released`, `Entered`, `Exited`, `Cancelled` — with precise coordinates for every finger/pointer on screen.

It is the foundation that [SkiaScene](skia-scene.md) builds on, but you can use it directly for any custom gesture or drawing use case.

## Platform Support

| Platform | Implementation |
| --- | --- |
| Android | `MotionEvent` processing with touch slop and boundary detection |
| iOS | Custom `UIGestureRecognizer` subclass (`TouchRecognizer`) |
| macOS Catalyst | Same as iOS (shared `.apple.cs` implementation) |
| Windows | WinUI Pointer events (`PointerPressed`, `PointerMoved`, `PointerReleased`, `PointerCanceled`, `PointerCaptureLost`) |

## XAML Namespace

```xml
xmlns:tracking="clr-namespace:Maui.FreakyEffects.TouchTracking;assembly=Maui.FreakyEffects"
```

## Effect Registration

Make sure `InitFreakyEffects()` is called in `MauiProgram.cs`. See [Getting Started](getting-started.md).

---

## TouchEffect

`TouchEffect` is a `RoutingEffect` that raises a `TouchAction` event for every pointer interaction on the attached view.

### Properties

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `Capture` | `bool` | `true` | When `true`, touch events are captured to this view even when the pointer moves outside it |

### Event

```csharp
public event TouchActionEventHandler TouchAction;
```

---

## TouchActionEventArgs

| Property | Type | Description |
| --- | --- | --- |
| `Id` | `long` | Unique identifier for this pointer. Stable across Pressed → Moved → Released for the same finger |
| `Type` | `TouchActionType` | The action that occurred |
| `Location` | `TouchTrackingPoint` | The pointer's position **relative to the attached view** in device-independent pixels |
| `IsInContact` | `bool` | `true` for Pressed and Moved; `false` for Released, Exited, and Cancelled |

### TouchActionType enum

| Value | When fired |
| --- | --- |
| `Pressed` | Pointer touches/clicks down |
| `Moved` | Pointer moves while in contact |
| `Released` | Pointer lifts |
| `Entered` | Pointer crosses into this view's bounds (when `Capture` is `false`) |
| `Exited` | Pointer crosses out of this view's bounds (when `Capture` is `false`) |
| `Cancelled` | Touch cancelled (e.g. incoming call, pointer capture lost) |

---

## Usage

### XAML

```xml
<ContentView x:Name="DrawSurface">
    <ContentView.Effects>
        <tracking:TouchEffect TouchAction="OnTouchAction" Capture="True" />
    </ContentView.Effects>
</ContentView>
```

### Code-Behind

```csharp
void OnTouchAction(object sender, TouchActionEventArgs args)
{
    switch (args.Type)
    {
        case TouchActionType.Pressed:
            StartPath(args.Id, args.Location);
            break;

        case TouchActionType.Moved:
            ExtendPath(args.Id, args.Location);
            break;

        case TouchActionType.Released:
        case TouchActionType.Cancelled:
            EndPath(args.Id);
            break;
    }
}
```

### Attaching in Code

```csharp
var effect = new TouchEffect { Capture = true };
effect.TouchAction += OnTouchAction;
myView.Effects.Add(effect);
```

---

## Multi-Touch Example

Each active finger has its own `Id`. Track them in a dictionary keyed by `Id` to support simultaneous touches:

```csharp
readonly Dictionary<long, SKPath> _paths = new();

void OnTouchAction(object sender, TouchActionEventArgs args)
{
    var point = new SKPoint((float)args.Location.X, (float)args.Location.Y);

    switch (args.Type)
    {
        case TouchActionType.Pressed:
            var path = new SKPath();
            path.MoveTo(point);
            _paths[args.Id] = path;
            break;

        case TouchActionType.Moved:
            if (_paths.TryGetValue(args.Id, out var p))
                p.LineTo(point);
            canvasView.InvalidateSurface();
            break;

        case TouchActionType.Released:
        case TouchActionType.Cancelled:
            _paths.Remove(args.Id);
            break;
    }
}
```

---

## Boundary Crossing

When `Capture` is `false`, the system tracks which view the pointer is over and fires `Entered`/`Exited` events as the pointer crosses view boundaries. This is useful for implementing hover effects or drag-and-drop targets.

```xml
<HorizontalStackLayout>
    <BoxView x:Name="ZoneA" Color="LightBlue" WidthRequest="150" HeightRequest="150">
        <BoxView.Effects>
            <tracking:TouchEffect TouchAction="OnZoneTouch" Capture="False" />
        </BoxView.Effects>
    </BoxView>
    <BoxView x:Name="ZoneB" Color="LightCoral" WidthRequest="150" HeightRequest="150">
        <BoxView.Effects>
            <tracking:TouchEffect TouchAction="OnZoneTouch" Capture="False" />
        </BoxView.Effects>
    </BoxView>
</HorizontalStackLayout>
```

```csharp
void OnZoneTouch(object sender, TouchActionEventArgs args)
{
    var zone = (BoxView)sender;
    zone.Color = args.Type switch
    {
        TouchActionType.Entered  => Colors.Yellow,
        TouchActionType.Exited   => zone == ZoneA ? Colors.LightBlue : Colors.LightCoral,
        _ => zone.Color
    };
}
```

---

## Supporting Types

### TouchTrackingPoint

A lightweight struct representing a 2D coordinate:

```csharp
public struct TouchTrackingPoint
{
    public float X { get; }
    public float Y { get; }
}
```

### TouchTrackingRect

A lightweight struct for rectangular regions:

```csharp
public struct TouchTrackingRect
{
    public float Left { get; }
    public float Top { get; }
    public float Width { get; }
    public float Height { get; }
    public bool Contains(TouchTrackingPoint point);
}
```
