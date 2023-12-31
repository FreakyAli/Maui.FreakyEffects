namespace Samples.ClickEffects;

public partial class ClickEffects : FreakyBaseContentPage
{
	public ClickEffects()
	{
		InitializeComponent();
		BindingContext = new ClickEffectsViewModel();
	}
}