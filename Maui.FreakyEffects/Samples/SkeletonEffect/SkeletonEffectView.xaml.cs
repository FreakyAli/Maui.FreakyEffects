using System.Timers;
using Timer = System.Timers.Timer;

namespace Samples.SkeletonEffect;

public partial class SkeletonEffectView : FreakyBaseContentPage
{
    public Timer timer;
    private SkeletonEffectViewModel viewModel;

    public SkeletonEffectView()
    {
        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
        BindingContext = viewModel = new SkeletonEffectViewModel();
        viewModel.SetPreviewItems();
        viewModel.IsBusy = true;
        timer = new Timer
        {
            Interval = 10000
        };
        timer.AutoReset = false;
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        viewModel.IsBusy = false;
        viewModel.LoadItems();
    }
}