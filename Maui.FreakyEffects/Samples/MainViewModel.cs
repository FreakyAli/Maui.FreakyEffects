using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Samples;

public class MainViewModel : BaseViewModel
{
    public ICommand ImageWasTappedCommand
    {
        get; set;
    }

    ObservableCollection<string> _suggestionItem;
    private ObservableCollection<string> items;

    public ObservableCollection<string> SuggestionItem
    {
        get => _suggestionItem;
        set
        {
            _suggestionItem = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> Items
    {
        get => items;
        set
        {
            items = value;
            OnPropertyChanged();
        }
    }

    public ICommand FreakyLongPressedCommand { get; set; }

    public MainViewModel()
    {
        Items = new ObservableCollection<string>
            {
                AppShell.skeleton
            };
    }
}
