using System.Runtime.CompilerServices;

namespace Samples;

public abstract class FreakyBaseViewModel : BindableObject
{
    private bool isBusy;

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

    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }
}