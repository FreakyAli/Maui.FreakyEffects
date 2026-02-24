using Maui.FreakyEffects.SkiaScene;
using SkiaSharp;
using Xunit;

namespace Maui.FreakyEffects.Tests.SkiaScene;

public class SKPointExtensionsTests
{
    [Fact]
    public void GetMagnitude_OfOriginPoint_ReturnsZero()
    {
        var point = new SKPoint(0f, 0f);

        Assert.Equal(0f, point.GetMagnitude());
    }

    [Fact]
    public void GetMagnitude_Of3_4_Triangle_Returns5()
    {
        // Classic 3-4-5 right triangle
        var point = new SKPoint(3f, 4f);

        Assert.Equal(5f, point.GetMagnitude(), precision: 5);
    }

    [Fact]
    public void GetMagnitude_OfNegativeCoordinates_ReturnsSameAsPositive()
    {
        var positive = new SKPoint(3f, 4f);
        var negative = new SKPoint(-3f, -4f);

        Assert.Equal(positive.GetMagnitude(), negative.GetMagnitude(), precision: 5);
    }

    [Fact]
    public void GetMagnitude_OfUnitVectorAlongX_Returns1()
    {
        var point = new SKPoint(1f, 0f);

        Assert.Equal(1f, point.GetMagnitude(), precision: 5);
    }

    [Fact]
    public void GetMagnitude_OfUnitVectorAlongY_Returns1()
    {
        var point = new SKPoint(0f, 1f);

        Assert.Equal(1f, point.GetMagnitude(), precision: 5);
    }

    [Fact]
    public void GetMagnitude_IsAlwaysNonNegative()
    {
        var points = new[]
        {
            new SKPoint(5f, -12f),
            new SKPoint(-8f, 6f),
            new SKPoint(-1f, -1f),
        };

        foreach (var point in points)
        {
            Assert.True(point.GetMagnitude() >= 0f);
        }
    }

    [Fact]
    public void GetMagnitude_Of5_12_Triangle_Returns13()
    {
        var point = new SKPoint(5f, 12f);

        Assert.Equal(13f, point.GetMagnitude(), precision: 4);
    }
}
