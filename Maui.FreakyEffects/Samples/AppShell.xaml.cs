namespace Samples;

public partial class AppShell : Shell
{
    internal const string skeleton = "Skeleton";
    internal const string touchTracking = "TouchTracking/SkiaScene";
    internal const string clickEffects = "Click effects";

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(skeleton, typeof(SkeletonEffect.SkeletonEffectView));
        Routing.RegisterRoute(touchTracking, typeof(TouchTracking.TouchTrackingView));
        Routing.RegisterRoute(clickEffects, typeof(ClickEffects.ClickEffects));
    }
}