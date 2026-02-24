using Maui.FreakyEffects.SkiaScene.TouchManipulation;
using Maui.FreakyEffects.TouchTracking;
using SkiaSharp;
using Xunit;

namespace Maui.FreakyEffects.Tests.SkiaScene;

public class PinchEventArgsTests
{
    [Fact]
    public void Constructor_SetsPreviousPoint()
    {
        var previous = new SKPoint(10f, 20f);
        var args = new PinchEventArgs(previous, SKPoint.Empty, SKPoint.Empty, TouchActionType.Moved);

        Assert.Equal(previous, args.PreviousPoint);
    }

    [Fact]
    public void Constructor_SetsNewPoint()
    {
        var newPoint = new SKPoint(50f, 60f);
        var args = new PinchEventArgs(SKPoint.Empty, newPoint, SKPoint.Empty, TouchActionType.Moved);

        Assert.Equal(newPoint, args.NewPoint);
    }

    [Fact]
    public void Constructor_SetsPivotPoint()
    {
        var pivot = new SKPoint(100f, 200f);
        var args = new PinchEventArgs(SKPoint.Empty, SKPoint.Empty, pivot, TouchActionType.Moved);

        Assert.Equal(pivot, args.PivotPoint);
    }

    [Fact]
    public void Constructor_SetsTouchActionType()
    {
        var args = new PinchEventArgs(SKPoint.Empty, SKPoint.Empty, SKPoint.Empty, TouchActionType.Released);

        Assert.Equal(TouchActionType.Released, args.TouchActionType);
    }

    [Fact]
    public void Constructor_AllPointsAreIndependent()
    {
        var previous = new SKPoint(1f, 2f);
        var newPoint = new SKPoint(3f, 4f);
        var pivot = new SKPoint(5f, 6f);

        var args = new PinchEventArgs(previous, newPoint, pivot, TouchActionType.Moved);

        Assert.Equal(previous, args.PreviousPoint);
        Assert.Equal(newPoint, args.NewPoint);
        Assert.Equal(pivot, args.PivotPoint);
    }
}
