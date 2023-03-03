using System;
using System.Collections.ObjectModel;

namespace Samples.SkeletonEffect;

public class ImageItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
}

public class SkeletonEffectViewModel
{
    public ObservableCollection<ImageItem> ImageItems { get; set; }

    public SkeletonEffectViewModel()
    {
        ImageItems = new ObservableCollection<ImageItem>
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
    }
}

