using BlueskyClient.Models;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class LocalImagePreview : UserControl
{
    public static readonly DependencyProperty ImageReferenceProperty = DependencyProperty.Register(
        nameof(ImageReference),
        typeof(FutureAccessImage),
        typeof(LocalImagePreview),
        new PropertyMetadata(null, OnImageChanged));

    private static async void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LocalImagePreview preview)
        {
            await preview.RefreshAsync();
        }
    }

    public LocalImagePreview()
    {
        this.InitializeComponent();
    }

    public FutureAccessImage? ImageReference
    {
        get => (FutureAccessImage?)GetValue(ImageReferenceProperty);
        set => SetValue(ImageReferenceProperty, value);
    }

    private async Task RefreshAsync()
    {
        if (ImageReference?.FutureAccessToken is not { Length: > 0 } token)
        {
            return;
        }

        StorageFile? file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
        if (file is not null)
        {
            using IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            BitmapImage bitmapImage = new()
            {
                DecodePixelWidth = 160
            };
            await bitmapImage.SetSourceAsync(fileStream);
            LocalImage.Source = bitmapImage;
        }
    }
}
