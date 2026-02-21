using Maui.FreakyEffects.Skeleton;
using Xunit;

namespace Maui.FreakyEffects.Tests.Skeleton;

public class AnimationConstructorTests
{
    [Fact]
    public void FadeAnimation_Constructor_SetsInterval()
    {
        var animation = new FadeAnimation(500, null);

        Assert.Equal(500u, animation.Interval);
    }

    [Fact]
    public void FadeAnimation_Constructor_WithNullParameter_UsesDefaultParameter()
    {
        var animation = new FadeAnimation(500, null);

        Assert.Equal(0.6, animation.Parameter);
    }

    [Fact]
    public void FadeAnimation_Constructor_WithExplicitParameter_UsesProvidedValue()
    {
        var animation = new FadeAnimation(300, 0.4);

        Assert.Equal(0.4, animation.Parameter);
    }

    [Fact]
    public void FadeAnimation_Constructor_WithZeroInterval_SetsZero()
    {
        var animation = new FadeAnimation(0, null);

        Assert.Equal(0u, animation.Interval);
    }

    [Fact]
    public void BeatAnimation_Constructor_SetsInterval()
    {
        var animation = new BeatAnimation(750, null);

        Assert.Equal(750u, animation.Interval);
    }

    [Fact]
    public void BeatAnimation_Constructor_WithNullParameter_UsesDefaultParameter()
    {
        var animation = new BeatAnimation(750, null);

        Assert.Equal(1.03, animation.Parameter);
    }

    [Fact]
    public void BeatAnimation_Constructor_WithExplicitParameter_UsesProvidedValue()
    {
        var animation = new BeatAnimation(400, 1.15);

        Assert.Equal(1.15, animation.Parameter);
    }

    [Fact]
    public void AnimationTypes_HasExpectedValues()
    {
        Assert.Equal(0, (int)AnimationTypes.None);
        Assert.Equal(1, (int)AnimationTypes.Beat);
        Assert.Equal(2, (int)AnimationTypes.Fade);
        Assert.Equal(3, (int)AnimationTypes.VerticalShake);
        Assert.Equal(4, (int)AnimationTypes.HorizontalShake);
    }

    [Fact]
    public void AnimationTypes_HasFiveMembers()
    {
        var values = Enum.GetValues<AnimationTypes>();

        Assert.Equal(5, values.Length);
    }
}
