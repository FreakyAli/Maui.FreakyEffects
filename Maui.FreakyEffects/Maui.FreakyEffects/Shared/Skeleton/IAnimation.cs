namespace Maui.FreakyEffects.Skeleton;

public interface IAnimation
{
    void Start(BindableObject bindable);
    void Stop(BindableObject bindable);
}