using Maui.FreakyEffects.SkiaScene.TouchManipulation;
using SkiaSharp;
using Xunit;

namespace Maui.FreakyEffects.Tests.SkiaScene;

public class TapEventArgsTests
{
    [Fact]
    public void Constructor_SetsViewPoint()
    {
        var viewPoint = new SKPoint(123f, 456f);
        var args = new TapEventArgs(viewPoint);

        Assert.Equal(viewPoint, args.ViewPoint);
    }

    [Fact]
    public void Constructor_WithOrigin_SetsZeroPoint()
    {
        var args = new TapEventArgs(SKPoint.Empty);

        Assert.Equal(SKPoint.Empty, args.ViewPoint);
    }

    [Fact]
    public void Constructor_WithNegativeCoordinates_StoresCorrectly()
    {
        var viewPoint = new SKPoint(-10f, -20f);
        var args = new TapEventArgs(viewPoint);

        Assert.Equal(-10f, args.ViewPoint.X);
        Assert.Equal(-20f, args.ViewPoint.Y);
    }
}
