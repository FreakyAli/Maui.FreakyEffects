using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Samples;

public abstract class FreakyBaseViewModel : BindableObject
{
    private bool isBusy;

    public ICommand BackButtonCommand { get; }

    public FreakyBaseViewModel()
    {
        BackButtonCommand = new Command(ExecuteOnBackButtonClicked);
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected virtual void ViewOnAppearing()
    {

    }

    protected virtual void ViewOnDisappearing()
    {

    }


    private async void ExecuteOnBackButtonClicked()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }
}