using System;
using System.Collections.ObjectModel;

namespace Samples.SkeletonEffect;

public class SkeletonEffectViewModel : FreakyBaseViewModel
{
    private bool isBusy;
    const string title = "";

    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }

    public ObservableCollection<ImageItem> ImageItems
    {
        get => imageItems;
        set => SetProperty(ref imageItems, value);
    }

    private ObservableCollection<ImageItem> previewItems = new ObservableCollection<ImageItem> {
         new ImageItem
            {
             Title= title,
             Description=title,
             ImageUrl= string.Empty
            },
            new ImageItem
            {
             Title= title,
             Description=title,
             ImageUrl= string.Empty
            },
            new ImageItem
            {
             Title= title,
             Description=title,
             ImageUrl= string.Empty
            },
            new ImageItem
            {
             Title= title,
             Description=title,
             ImageUrl= string.Empty
            }
    };

    private ObservableCollection<ImageItem> loadItems = new ObservableCollection<ImageItem>
        {
            new ImageItem
            {
                Title = "Image 1",
                Description = "This is image 1",
                ImageUrl = "https://picsum.photos/200/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 2",
                Description = "This is image 2",
                ImageUrl = "https://picsum.photos/201/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 3",
                Description = "This is image 3",
                ImageUrl = "https://picsum.photos/202/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 4",
                Description = "This is image 4",
                ImageUrl = "https://picsum.photos/203/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 5",
                Description = "This is image 5",
                ImageUrl = "https://picsum.photos/204/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 6",
                Description = "This is image 6",
                ImageUrl = "https://picsum.photos/205/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 7",
                Description = "This is image 7",
                ImageUrl = "https://picsum.photos/206/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 8",
                Description = "This is image 8",
                ImageUrl = "https://picsum.photos/207/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 9",
                Description = "This is image 9",
                ImageUrl = "https://picsum.photos/208/300.jpg"
            },
            new ImageItem
            {
                Title = "Image 10",
                Description = "This is image 10",
                ImageUrl = "https://picsum.photos/209/300.jpg"
            }
        };
    private ObservableCollection<ImageItem> imageItems;

    public SkeletonEffectViewModel()
    {

    }

    internal void SetPreviewItems()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        ImageItems = previewItems);
    }

    internal void LoadItems()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        ImageItems = loadItems);
    }
}