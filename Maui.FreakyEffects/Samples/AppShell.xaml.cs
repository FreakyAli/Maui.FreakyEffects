namespace Samples;

public partial class AppShell : Shell
{
    internal const string skeleton = "Skeleton";
    internal const string touchTracking = "TouchTracking/SkiaScene";


    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(skeleton, typeof(SkeletonEffect.SkeletonEffectView));
        Routing.RegisterRoute(touchTracking, typeof(TouchTracking.TouchTrackingView));
    }
}

