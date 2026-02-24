using Maui.FreakyEffects.TouchTracking;
using Xunit;

namespace Maui.FreakyEffects.Tests.TouchTracking;

public class TouchTrackingPointTests
{
    [Fact]
    public void Constructor_SetsXAndY()
    {
        var point = new TouchTrackingPoint(3.5f, 7.2f);

        Assert.Equal(3.5f, point.X);
        Assert.Equal(7.2f, point.Y);
    }

    [Fact]
    public void Constructor_WithZeroValues_SetsZero()
    {
        var point = new TouchTrackingPoint(0f, 0f);

        Assert.Equal(0f, point.X);
        Assert.Equal(0f, point.Y);
    }

    [Fact]
    public void Constructor_WithNegativeValues_StoresCorrectly()
    {
        var point = new TouchTrackingPoint(-10f, -20f);

        Assert.Equal(-10f, point.X);
        Assert.Equal(-20f, point.Y);
    }

    [Fact]
    public void X_CanBeSetAfterConstruction()
    {
        var point = new TouchTrackingPoint(1f, 2f);
        point.X = 99f;

        Assert.Equal(99f, point.X);
    }

    [Fact]
    public void Y_CanBeSetAfterConstruction()
    {
        var point = new TouchTrackingPoint(1f, 2f);
        point.Y = 88f;

        Assert.Equal(88f, point.Y);
    }
}
