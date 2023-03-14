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
        BindingContext = viewModel = new SkeletonEffectViewModel();
        viewModel.IsBusy = true;
        timer = new Timer
        {
            Interval = 5000
        };
        timer.AutoReset = false;
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        viewModel.IsBusy = false;
    }

    async void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        var canOpen = await Launcher.CanOpenAsync(Constants.ProfileUrl);
        if (canOpen)
        {
            await Launcher.OpenAsync(Constants.ProfileUrl);
        }
    }
}