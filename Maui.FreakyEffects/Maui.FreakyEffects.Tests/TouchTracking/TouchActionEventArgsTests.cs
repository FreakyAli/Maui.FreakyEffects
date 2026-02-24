using Maui.FreakyEffects.TouchTracking;
using Xunit;

namespace Maui.FreakyEffects.Tests.TouchTracking;

public class TouchActionEventArgsTests
{
    [Fact]
    public void Constructor_SetsId()
    {
        var location = new TouchTrackingPoint(10f, 20f);
        var args = new TouchActionEventArgs(42L, TouchActionType.Pressed, location, true);

        Assert.Equal(42L, args.Id);
    }

    [Fact]
    public void Constructor_SetsType()
    {
        var location = new TouchTrackingPoint(0f, 0f);
        var args = new TouchActionEventArgs(1L, TouchActionType.Moved, location, false);

        Assert.Equal(TouchActionType.Moved, args.Type);
    }

    [Fact]
    public void Constructor_SetsLocation()
    {
        var location = new TouchTrackingPoint(5f, 15f);
        var args = new TouchActionEventArgs(1L, TouchActionType.Pressed, location, false);

        Assert.Equal(5f, args.Location.X);
        Assert.Equal(15f, args.Location.Y);
    }

    [Fact]
    public void Constructor_SetsIsInContact_True()
    {
        var location = new TouchTrackingPoint(0f, 0f);
        var args = new TouchActionEventArgs(1L, TouchActionType.Pressed, location, true);

        Assert.True(args.IsInContact);
    }

    [Fact]
    public void Constructor_SetsIsInContact_False()
    {
        var location = new TouchTrackingPoint(0f, 0f);
        var args = new TouchActionEventArgs(1L, TouchActionType.Released, location, false);

        Assert.False(args.IsInContact);
    }

    [Theory]
    [InlineData(TouchActionType.Entered)]
    [InlineData(TouchActionType.Pressed)]
    [InlineData(TouchActionType.Moved)]
    [InlineData(TouchActionType.Released)]
    [InlineData(TouchActionType.Cancelled)]
    [InlineData(TouchActionType.Exited)]
    public void Constructor_SupportsAllTouchActionTypes(TouchActionType type)
    {
        var location = new TouchTrackingPoint(0f, 0f);
        var args = new TouchActionEventArgs(1L, type, location, false);

        Assert.Equal(type, args.Type);
    }
}
