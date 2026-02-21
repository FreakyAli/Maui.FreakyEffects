using Maui.FreakyEffects.TouchTracking;
using Xunit;

namespace Maui.FreakyEffects.Tests.TouchTracking;

public class TouchTrackingRectTests
{
    [Fact]
    public void Constructor_SetsLeft()
    {
        var rect = new TouchTrackingRect(10f, 20f, 30f, 40f);

        Assert.Equal(10f, rect.Left);
    }

    [Fact]
    public void Constructor_SetsTop()
    {
        var rect = new TouchTrackingRect(10f, 20f, 30f, 40f);

        Assert.Equal(20f, rect.Top);
    }

    [Fact]
    public void Constructor_SetsRight()
    {
        var rect = new TouchTrackingRect(10f, 20f, 30f, 40f);

        Assert.Equal(30f, rect.Right);
    }

    [Fact]
    public void Constructor_SetsBottom()
    {
        var rect = new TouchTrackingRect(10f, 20f, 30f, 40f);

        Assert.Equal(40f, rect.Bottom);
    }

    [Fact]
    public void Contains_ReturnsTrueForPointInsideRect()
    {
        var rect = new TouchTrackingRect(0f, 0f, 100f, 100f);

        Assert.True(rect.Contains(50f, 50f));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointLeftOfRect()
    {
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.False(rect.Contains(5f, 30f));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointRightOfRect()
    {
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.False(rect.Contains(60f, 30f));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointAboveRect()
    {
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.False(rect.Contains(30f, 5f));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointBelowRect()
    {
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.False(rect.Contains(30f, 60f));
    }

    [Fact]
    public void Contains_ReturnsTrueForPointAtLeftBoundary()
    {
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.True(rect.Contains(10f, 30f));
    }

    [Fact]
    public void Contains_ReturnsTrueForPointAtTopBoundary()
    {
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.True(rect.Contains(30f, 10f));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointAtRightBoundary()
    {
        // Right boundary is exclusive (x < Right)
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.False(rect.Contains(50f, 30f));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointAtBottomBoundary()
    {
        // Bottom boundary is exclusive (y < Bottom)
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);

        Assert.False(rect.Contains(30f, 50f));
    }

    [Fact]
    public void Contains_WithTouchTrackingPoint_ReturnsTrueForPointInsideRect()
    {
        var rect = new TouchTrackingRect(0f, 0f, 100f, 100f);
        var point = new TouchTrackingPoint(50f, 50f);

        Assert.True(rect.Contains(point));
    }

    [Fact]
    public void Contains_WithTouchTrackingPoint_ReturnsFalseForPointOutsideRect()
    {
        var rect = new TouchTrackingRect(10f, 10f, 50f, 50f);
        var point = new TouchTrackingPoint(5f, 5f);

        Assert.False(rect.Contains(point));
    }
}
