# SkiaScene

SkiaScene is a hardware-accelerated 2D scene system built on [SkiaSharp](https://github.com/mono/SkiaSharp). It wraps a `SKCanvasView` with a full gesture recogniser and a matrix-based transformation engine, giving you pinch-to-zoom, pan, rotation, tap, and double-tap out of the box.

## Platform Support

All platforms — iOS, Android, macOS Catalyst, Windows. SkiaScene is implemented entirely in shared code via SkiaSharp; there are no platform-specific files.

## Dependencies

SkiaSharp is included transitively through the `FreakyEffects` NuGet package. No additional package references are needed.

---

## Core Concepts

### SKScene

`SKScene` owns an `SKMatrix` that accumulates all user transformations (translate, scale, rotate). Every gesture updates the matrix, which is then applied to the canvas before your renderer draws.

Key properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `MinScale` | `float` | `1/8` (0.125) | Minimum allowed zoom level |
| `MaxScale` | `float` | `8` | Maximum allowed zoom level |
| `CenterBoundary` | `SKRect` | `empty` | If set, panning is constrained so the scene centre stays inside this rect |
| `ScreenCenter` | `SKPoint` | — | The view-space point treated as the scene centre (typically the canvas midpoint) |

Key methods:

| Method | Description |
| --- | --- |
| `Render(SKCanvas)` | Apply the current matrix and call the renderer. Call this inside `PaintSurface`. |
| `MoveByVector(SKPoint)` | Translate by a vector in view space |
| `MoveToPoint(SKPoint)` | Move so the specified canvas point is at `ScreenCenter` |
| `ZoomByScaleFactor(SKPoint, float)` | Zoom around a point by a multiplicative factor, respecting `MinScale`/`MaxScale` |
| `Zoom(SKPoint, float)` | Zoom to an absolute scale value |
| `Rotate(SKPoint, float)` | Rotate to an absolute angle (radians) around a point |
| `RotateByRadiansDelta(SKPoint, float)` | Add a rotation delta |
| `GetCenter()` | Returns the canvas-space point currently at `ScreenCenter` |
| `GetScale()` | Returns the current zoom level |
| `GetAngleInRadians()` | Returns the current rotation in the range [-π, π] |
| `GetCanvasPointFromViewPoint(SKPoint)` | Converts a view-space point to canvas-space (inverse matrix) |

### ISKSceneRenderer

Implement this interface to provide your drawing logic:

```csharp
public interface ISKSceneRenderer
{
    void Render(SKCanvas canvas, float angleInRadians, SKPoint center, float scale);
}
```

The scene calls `Render` after applying the transformation matrix, so you draw in your own coordinate space and the scene handles all zooming/panning/rotation.

### TouchGestureRecognizer

`TouchGestureRecognizer` sits on top of the Touch Tracking layer and recognises higher-level gestures from raw touch events. It fires:

| Event | Args type | When |
| --- | --- | --- |
| `Tap` | `TapEventArgs` | Single tap (short press, not moved) |
| `DoubleTap` | `TapEventArgs` | Two taps within 320 ms |
| `Pan` | `PanEventArgs` | One-finger drag |
| `Pinch` | `PinchEventArgs` | Two-finger pinch/spread (scale + rotation) |

### SceneGestureResponder

`SceneGestureResponder` connects a `TouchGestureRecognizer` to an `SKScene`, forwarding gesture events as matrix transformations. Use it when you do not need to intercept individual events.

`SceneGestureRenderingResponder` extends this with automatic canvas invalidation after each gesture.

---

## Basic Usage

### 1. Implement ISKSceneRenderer

```csharp
public class MyRenderer : ISKSceneRenderer
{
    public void Render(SKCanvas canvas, float angleInRadians, SKPoint center, float scale)
    {
        canvas.Clear(SKColors.White);

        using var paint = new SKPaint
        {
            Color = SKColors.DeepSkyBlue,
            IsAntialias = true
        };

        canvas.DrawCircle(0, 0, 100, paint);
        canvas.DrawLine(-200, 0, 200, 0, paint);
        canvas.DrawLine(0, -200, 0, 200, paint);
    }
}
```

### 2. Wire up the Scene in Code-Behind

```csharp
public partial class MyPage : ContentPage
{
    private SKScene _scene;
    private TouchGestureRecognizer _gestureRecognizer;
    private SceneGestureRenderingResponder _responder;
    private readonly MyRenderer _renderer = new MyRenderer();

    public MyPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _scene = new SKScene(_renderer)
        {
            MinScale = 0.1f,
            MaxScale = 10f
        };

        _gestureRecognizer = new TouchGestureRecognizer();

        _responder = new SceneGestureRenderingResponder(
            () => canvasView.InvalidateSurface(),
            _scene,
            _gestureRecognizer);

        _responder.TouchManipulationMode = TouchManipulationMode.IsotropicScale;
        _responder.MaxDurationBetweenTapsMs = 320;

        // Attach touch tracking
        var touchEffect = new TouchTracking.TouchEffect { Capture = true };
        touchEffect.TouchAction += OnTouchAction;
        canvasView.Effects.Add(touchEffect);
    }

    void OnTouchAction(object sender, TouchActionEventArgs args)
    {
        var point = new SKPoint((float)args.Location.X, (float)args.Location.Y);
        _gestureRecognizer.ProcessTouchEvent(args.Id, args.Type, point);
    }

    void OnCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        var canvas = args.Surface.Canvas;
        var info = args.Info;

        _scene.ScreenCenter = new SKPoint(info.Width / 2f, info.Height / 2f);
        _scene.Render(canvas);
    }
}
```

### 3. XAML Canvas

```xml
<skia:SKCanvasView
    x:Name="canvasView"
    PaintSurface="OnCanvasPaintSurface" />
```

---

## Gesture Events

### TapEventArgs

```csharp
public class TapEventArgs : EventArgs
{
    public SKPoint ViewPoint { get; }  // Tap location in view space
}
```

### PanEventArgs

```csharp
public class PanEventArgs : EventArgs
{
    public SKPoint PreviousPoint { get; }
    public SKPoint NewPoint { get; }
}
```

### PinchEventArgs

```csharp
public class PinchEventArgs : EventArgs
{
    public SKPoint FirstPoint { get; }   // First finger position
    public SKPoint SecondPoint { get; }  // Second finger position
    public float Scale { get; }          // Ratio of new pinch distance to old
}
```

---

## Reacting to Taps

Subscribe to the gesture recogniser's events to handle taps separately from zoom/pan:

```csharp
_gestureRecognizer.Tap += (sender, e) =>
{
    // Convert tap to canvas coordinates
    var canvasPoint = _scene.GetCanvasPointFromViewPoint(e.ViewPoint);
    Console.WriteLine($"Tapped canvas point: {canvasPoint}");
};

_gestureRecognizer.DoubleTap += (sender, e) =>
{
    // Double-tap to reset zoom
    _scene.Zoom(e.ViewPoint, 1f);
    canvasView.InvalidateSurface();
};
```

---

## TouchManipulationMode

Controls how pinch gestures are interpreted:

| Value | Description |
| --- | --- |
| `None` | Gestures are ignored |
| `PanOnly` | Only panning; no scale or rotation |
| `IsotropicScale` | Pan + uniform scale (no rotation) |
| `AnisotropicScale` | Pan + independent X/Y scale |
| `ScaleRotate` | Pan + uniform scale + rotation |

```csharp
_responder.TouchManipulationMode = TouchManipulationMode.ScaleRotate;
```

---

## Constraining Pan

Set `CenterBoundary` to keep the scene centre inside a rect (useful for maps or large images):

```csharp
_scene.CenterBoundary = new SKRect(-500, -500, 500, 500);
```

If the user tries to pan beyond the boundary the translation is rolled back automatically.
