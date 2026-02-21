using Maui.FreakyEffects.SkiaScene.TouchManipulation;
using Maui.FreakyEffects.TouchTracking;
using SkiaSharp;
using Xunit;

namespace Maui.FreakyEffects.Tests.SkiaScene;

public class PanEventArgsTests
{
    [Fact]
    public void Constructor_SetsPreviousPoint()
    {
        var previous = new SKPoint(10f, 20f);
        var current = new SKPoint(30f, 40f);
        var args = new PanEventArgs(previous, current, TouchActionType.Moved);

        Assert.Equal(previous, args.PreviousPoint);
    }

    [Fact]
    public void Constructor_SetsNewPoint()
    {
        var previous = new SKPoint(10f, 20f);
        var current = new SKPoint(30f, 40f);
        var args = new PanEventArgs(previous, current, TouchActionType.Moved);

        Assert.Equal(current, args.NewPoint);
    }

    [Fact]
    public void Constructor_SetsTouchActionType()
    {
        var args = new PanEventArgs(SKPoint.Empty, SKPoint.Empty, TouchActionType.Released);

        Assert.Equal(TouchActionType.Released, args.TouchActionType);
    }

    [Theory]
    [InlineData(TouchActionType.Moved)]
    [InlineData(TouchActionType.Released)]
    public void Constructor_SupportsRelevantActionTypes(TouchActionType type)
    {
        var args = new PanEventArgs(SKPoint.Empty, SKPoint.Empty, type);

        Assert.Equal(type, args.TouchActionType);
    }
}
