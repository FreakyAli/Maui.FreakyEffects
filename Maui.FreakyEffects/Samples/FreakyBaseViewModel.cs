using System.Runtime.CompilerServices;

namespace Samples;

public abstract class FreakyBaseViewModel : BindableObject
{
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

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public bool IsContextVisible
    {
        get => (bool)GetValue(IsContextVisibleProperty);
        set => SetValue(IsContextVisibleProperty, value);
    }

    public bool IsBackButtonVisible
    {
        get => (bool)GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    public bool HasAddButton
    {
        get => (bool)GetValue(HasAddButtonProperty);
        set => SetValue(HasAddButtonProperty, value);
    }

    public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create
           (
           propertyName: nameof(HeaderText),
           returnType: typeof(string),
           declaringType: typeof(FreakyBaseContentPage),
           defaultValue: string.Empty
           );

    public static readonly BindableProperty IsContextVisibleProperty = BindableProperty.Create
        (
        propertyName: nameof(IsContextVisible),
        returnType: typeof(bool),
        declaringType: typeof(FreakyBaseContentPage),
        defaultValue: false
        );

    public static readonly BindableProperty IsBackButtonVisibleProperty = BindableProperty.Create
        (
        propertyName: nameof(IsBackButtonVisible),
        returnType: typeof(bool),
        declaringType: typeof(FreakyBaseContentPage),
        defaultValue: true
        );

    public static readonly BindableProperty HasAddButtonProperty = BindableProperty.Create
       (
       propertyName: nameof(HasAddButton),
       returnType: typeof(bool),
       declaringType: typeof(FreakyBaseContentPage),
       defaultValue: false
       );

}