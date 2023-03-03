using System.Text.Json;

namespace Samples.SkeletonEffect;

public partial class SkeletonEffectView : ContentPage
{
    public SkeletonEffectView()
    {
        InitializeComponent();
        BindingContext = new SkeletonEffectViewModel();
    }


}