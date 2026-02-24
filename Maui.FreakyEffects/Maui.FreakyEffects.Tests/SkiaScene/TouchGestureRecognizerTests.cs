using Maui.FreakyEffects.SkiaScene.TouchManipulation;
using Maui.FreakyEffects.TouchTracking;
using SkiaSharp;
using Xunit;

namespace Maui.FreakyEffects.Tests.SkiaScene;

public class TouchGestureRecognizerTests
{
    [Fact]
    public void ProcessTouchEvent_Pressed_DoesNotFirePanOrPinch()
    {
        var recognizer = new TouchGestureRecognizer();
        bool panFired = false;
        bool pinchFired = false;
        recognizer.OnPan += (_, _) => panFired = true;
        recognizer.OnPinch += (_, _) => pinchFired = true;

        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(0, 0));

        Assert.False(panFired);
        Assert.False(pinchFired);
    }

    [Fact]
    public void ProcessTouchEvent_MovedWithOneTouch_FiresOnPan()
    {
        var recognizer = new TouchGestureRecognizer();
        PanEventArgs? receivedArgs = null;
        recognizer.OnPan += (_, args) => receivedArgs = args;

        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(0, 0));
        recognizer.ProcessTouchEvent(1, TouchActionType.Moved, new SKPoint(10, 20));

        Assert.NotNull(receivedArgs);
        Assert.Equal(TouchActionType.Moved, receivedArgs.TouchActionType);
        Assert.Equal(new SKPoint(0, 0), receivedArgs.PreviousPoint);
        Assert.Equal(new SKPoint(10, 20), receivedArgs.NewPoint);
    }

    [Fact]
    public void ProcessTouchEvent_MovedWithTwoTouches_FiresOnPinch()
    {
        var recognizer = new TouchGestureRecognizer();
        PinchEventArgs? receivedArgs = null;
        recognizer.OnPinch += (_, args) => receivedArgs = args;

        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(0, 0));
        recognizer.ProcessTouchEvent(2, TouchActionType.Pressed, new SKPoint(100, 100));
        recognizer.ProcessTouchEvent(1, TouchActionType.Moved, new SKPoint(10, 10));

        Assert.NotNull(receivedArgs);
        Assert.Equal(TouchActionType.Moved, receivedArgs.TouchActionType);
    }

    [Fact]
    public void ProcessTouchEvent_MovedWithoutPriorPress_DoesNotFirePan()
    {
        var recognizer = new TouchGestureRecognizer();
        bool panFired = false;
        recognizer.OnPan += (_, _) => panFired = true;

        // Move without prior press
        recognizer.ProcessTouchEvent(1, TouchActionType.Moved, new SKPoint(10, 20));

        Assert.False(panFired);
    }

    [Fact]
    public void ProcessTouchEvent_Released_RemovesTouchFromTracking()
    {
        var recognizer = new TouchGestureRecognizer();
        int panCount = 0;
        recognizer.OnPan += (_, _) => panCount++;

        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(0, 0));
        recognizer.ProcessTouchEvent(1, TouchActionType.Released, new SKPoint(5, 5));

        // After release, a move with the same id should do nothing
        recognizer.ProcessTouchEvent(1, TouchActionType.Moved, new SKPoint(10, 10));

        Assert.Equal(0, panCount); // Pan fires on Released (1 touch), but not on Move after release
    }

    [Fact]
    public void ProcessTouchEvent_ReleasedWithoutPriorPress_DoesNotThrow()
    {
        var recognizer = new TouchGestureRecognizer();

        var exception = Record.Exception(() =>
            recognizer.ProcessTouchEvent(1, TouchActionType.Released, new SKPoint(5, 5)));

        Assert.Null(exception);
    }

    [Fact]
    public void ProcessTouchEvent_Cancelled_RemovesTouchFromTracking()
    {
        var recognizer = new TouchGestureRecognizer();
        int panCount = 0;
        recognizer.OnPan += (_, _) => panCount++;

        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(0, 0));
        recognizer.ProcessTouchEvent(1, TouchActionType.Cancelled, new SKPoint(0, 0));

        // After cancel, a move with the same id should do nothing
        recognizer.ProcessTouchEvent(1, TouchActionType.Moved, new SKPoint(10, 10));

        Assert.Equal(0, panCount);
    }

    [Fact]
    public void ProcessTouchEvent_CancelledWithoutPriorPress_DoesNotThrow()
    {
        var recognizer = new TouchGestureRecognizer();

        var exception = Record.Exception(() =>
            recognizer.ProcessTouchEvent(1, TouchActionType.Cancelled, new SKPoint(0, 0)));

        Assert.Null(exception);
    }

    [Fact]
    public void ProcessTouchEvent_DuplicatePressed_DoesNotThrow()
    {
        var recognizer = new TouchGestureRecognizer();

        var exception = Record.Exception(() =>
        {
            recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(0, 0));
            recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(5, 5));
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ProcessTouchEvent_DuplicatePressed_UpdatesExistingEntry()
    {
        var recognizer = new TouchGestureRecognizer();
        PanEventArgs? receivedArgs = null;
        recognizer.OnPan += (_, args) => receivedArgs = args;

        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(0, 0));
        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(50, 50));
        recognizer.ProcessTouchEvent(1, TouchActionType.Moved, new SKPoint(60, 60));

        Assert.NotNull(receivedArgs);
        // Previous point should be the last press location (50, 50)
        Assert.Equal(new SKPoint(50, 50), receivedArgs.PreviousPoint);
    }

    [Fact]
    public void ProcessTouchEvent_ReleasedWithMinimalMove_FiresOnTap()
    {
        var recognizer = new TouchGestureRecognizer();
        bool tapFired = false;
        recognizer.OnTap += (_, _) => tapFired = true;

        recognizer.ProcessTouchEvent(1, TouchActionType.Pressed, new SKPoint(100, 100));
        recognizer.ProcessTouchEvent(1, TouchActionType.Released, new SKPoint(100, 100));

        Assert.True(tapFired);
    }
}
