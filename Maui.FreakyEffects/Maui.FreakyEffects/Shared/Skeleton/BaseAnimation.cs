namespace Maui.FreakyEffects.Skeleton;
[AcceptEmptyServiceProvider]
public abstract class BaseAnimation : IAnimation
{
    public uint Interval { get; set; }

    private double? _Parameter;
    public double Parameter
    {
        get => _Parameter ??= 0.6;
        set => _Parameter = value;
    }

    protected abstract Task<bool> Animate(BindableObject bindable);
    protected abstract Task StopAnimation(BindableObject bindable);

    public void Start(BindableObject bindable)
    {
        Task.Run(async () => { await this.Run(bindable); });
    }

    public void Stop(BindableObject bindable)
    {
        Task.Run(async () => { await this.StopAnimation(bindable); });
    }

    private async Task<bool> Run(BindableObject bindable)
    {
        if (SkeletonEffect.GetCancelAnimation(bindable))
        {
            SkeletonEffect.SetAnimating(bindable, false);
            return false;
        }
        else
        {
            SkeletonEffect.SetAnimating(bindable, true);
            await Animate(bindable);
            return await Run(bindable);
        }
    }
}