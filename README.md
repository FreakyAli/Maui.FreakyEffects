# Maui.FreakyEffects.SkiaScene

Collection of lightweight libraries which can be used to simplify manuplation with SkiaSharp graphic objects. 

Supported platforms are .NET Standard 1.3, UWP, Xamarin.iOS, Xamarin.Android.

## Libraries

### SkiaScene
```
Install-Package SkiaScene
```
Implemented as .NET Standard 1.3 library.

Basic class which allows controlling `SKCanvas` transformations without the need to directly manipulate underlaying Matrix.
First you need to implement `Render` method of `ISKSceneRenderer` interface, where you define all your drawing logic on `SKCanvas`.
Then create the `SKScene` instance:

```csharp
//Create scene
ISKSceneRenderer myRenderer = new MyRenderer(); //user defined instance 
scene = new SKScene(myRenderer);

//In your paint method
scene.Render(canvas); //Pass canvas from SKPaintSurfaceEventArgs

//Scene manipulation
scene.MoveByVector(new SKPoint(10, 0)); //Move by 10 units to the right independently from current rotation and zoom
scene.ZoomByScaleFactor(scene.GetCenter(), 1.2f); //Zoom to the center
scene.RotateByRadiansDelta(scene.GetCenter(), 0.1f); //Rotate around center
canvasView.InvalidateSurface(); //Force to repaint
```

### TouchTracking
```
Install-Package TouchTracking
```
Implemented as .NET Standard 1.3 and platform specific libraries.

TouchTracking provides unified API for multi-touch gestures on Android, iOS and UWP. It can be used without Xamarin.Forms. 
Basic principles are described in Xamarin Documentation https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/effects/touch-tracking/

Usage is similar on each platform. 

Android example:

```csharp
//Initialization
canvasView = FindViewById<SKCanvasView>(Resource.Id.canvasView); //Get SKCanvasView
touchHandler = new TouchHandler() { UseTouchSlop = true };
touchHandler.RegisterEvents(canvasView); //Pass View to the touch handler
touchHandler.TouchAction += OnTouch; //Listen to the touch gestures

void OnTouch(object sender, TouchActionEventArgs args) {
    var point = args.Location; //Point location
    var type = args.Type; //Entered, Pressed, Moved ... etc.
    //... do something
}
```

### TouchTracking.Forms
```
Install-Package TouchTracking.Forms
```
Implemented as .NET Standard 1.3 and platform specific libraries.

Same functionality as TouchTracking library but can be consumed in Xamarin.Forms as an Effect called TouchEffect.

```
xmlns:tt="clr-namespace:TouchTracking.Forms;assembly=TouchTracking.Forms"

<Grid>
    <views:SKCanvasView x:Name="canvasView" />
    <Grid.Effects>
        <tt:TouchEffect Capture="True" TouchAction="OnTouch" />
    </Grid.Effects>
</Grid>
```



**Important:** Right now, there is an issue on iOS in Xamarin.Forms, where routing effect resolution fails silently. To fix it, you must include this line in your AppDelegate FinishedLaunching:

```csharp
var _ = new TouchTracking.Forms.iOS.TouchEffect();
```

On UWP there is a similar problem in the Release mode where you need to provide explicit Assembly parameter to your Xamarin.Forms.Init method in App.Xaml.cs inside UWP project:

```csharp
Xamarin.Forms.Forms.Init(e, new[] { typeof(TouchTracking.Forms.UWP.TouchEffect).Assembly });
```

### SkiaScene.TouchManipulations
```
Install-Package SkiaScene.TouchManipulations
```
Implemented as .NET Standard 1.3 library.

Combines SkiaScene and TouchTracking libraries to detect and respond to the tap, pinch and pan gestures. Most of the functionality is described in Xamarin Documentation https://developer.xamarin.com/guides/xamarin-forms/advanced/skiasharp/transforms/touch/

`TouchGestureRecognizer` recieves touch event info in 'ProcessTouchEvent' method and fires concrete gesture event.

`SceneGestureResponder` subscribes to the events of `TouchGestureRecognizer` and executes correct actions in underlying `SKScene`.

`SceneGestureRenderingResponder` inherits from `SceneGestureResponder` and additionally controls frequency of calling `InvalidateSurface` method through `MaxFramesPerSecond` property.
1.
To determine with precision; to mark out with distinctness; to ascertain or exhibit clearly.
2.
To express the essential nature of something.
3.
To state the meaning of a word, phrase, sign, or symbol.
