using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using BlueskyClient.Services;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class PostEmbeds : UserControl
{
    private readonly IImageViewerService _imageViewerService;
    private readonly ILocalizer _localizer;

    public static readonly DependencyProperty EmbedProperty = DependencyProperty.Register(
        nameof(Embed),
        typeof(PostEmbed),
        typeof(PostEmbeds),
        new PropertyMetadata(null, OnEmbedChanged));

    private static void OnEmbedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PostEmbeds p)
        {
            p.Bindings.Update();
            p.TryLoadVideo();
        }
    }

    public PostEmbeds()
    {
        this.InitializeComponent();
        _imageViewerService = App.Services.GetRequiredService<IImageViewerService>();
        _localizer = App.Services.GetRequiredService<ILocalizer>();
    }

    public PostEmbed? Embed
    {
        get => (PostEmbed?)GetValue(EmbedProperty);
        set => SetValue(EmbedProperty, value);
    }

    private bool IsStarterPack => Embed?.Record?.Record?.Type.GetRecordType() is RecordType.StarterPack;

    private string StarterPackName => Embed?.Record?.Record?.Name ?? string.Empty;

    private string StarterPackCreatedByLine => _localizer.GetString("StarterPackByText", Embed?.Record?.Creator?.AtHandle ?? string.Empty);

    private string StarterPackDescription => Embed?.Record?.Record?.Description ?? string.Empty;

    private bool IsVideo => Embed?.Playlist is { Length: > 0 };

    private bool IsExternalUrl => Embed?.External?.Uri is { Length: > 0 } url && !url.Contains(".gif");

    private string ExternalRootUri => Embed?.External?.RootUri ?? string.Empty;

    private bool IsGif => Embed?.External?.Uri is { Length: > 0 } url && url.Contains(".gif");

    private string GifUrl => Embed?.External?.Uri is { Length: > 0 } url && Uri.IsWellFormedUriString(url, UriKind.Absolute)
        ? url
        : "http://localhost";

    private string ExternalThumb => Embed?.External?.Thumb is { Length: > 0 } thumbUrl && Uri.IsWellFormedUriString(thumbUrl, UriKind.Absolute)
        ? thumbUrl
        : "ms-appx:///Assets/ExternalThumbnailFallback.png";

    private string ExternalTitle => Embed?.External?.Title is { Length: > 0 } title
        ? title
        : Embed?.External?.Uri ?? string.Empty;

    private string ExternalDescription => Embed?.External?.Description is { Length: > 0 } description
        ? description
        : string.Empty;

    private bool IsSingleImageEmbed => Embed?.Images?.Length == 1;

    private bool IsMultiImageEmbed => Embed?.Images?.Length > 1;

    public IReadOnlyList<ImageEmbed> MultiImages => Embed?.Images ?? [];

    private ImageEmbed? SingleImage => Embed?.Images is [ImageEmbed image, ..]
        ? image
        : null;

    private double SingleImageMaxWidth =>
        SingleImage?.AspectRatio is { Height: double height, Width: double width } && height > width
        ? 300
        : double.PositiveInfinity;

    private void TryLoadVideo()
    {
        if (Embed?.Playlist is { Length: > 0 } sourceUrl && 
            Uri.TryCreate(sourceUrl, UriKind.Absolute, out Uri videoSource))
        {
            VideoPlayer.Source = MediaSource.CreateFromUri(videoSource);

            if (Embed?.Thumbnail is { Length: > 0 } thumbUrl && 
                Uri.TryCreate(thumbUrl, UriKind.Absolute, out Uri thumbSource))
            {
                VideoPlayer.PosterSource = new BitmapImage(thumbSource);
            }
        }
    }

    private async void OnExternalUrlClicked(object sender, RoutedEventArgs e)
    {
        if (Embed?.External?.Uri is string { Length: > 0 } uri &&
            Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
        {
            await Launcher.LaunchUriAsync(result);
        }
    }

    private void OnGridViewImageClicked(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not ImageEmbed imageClicked)
        {
            return;
        }

        var index = 0;

        foreach (var image in MultiImages)
        {
            if (image == imageClicked)
            {
                break;
            }

            index++;
        }

        if (index < MultiImages.Count)
        {
            _imageViewerService.RequestImageViewer(MultiImages, launchIndex: index);
        }
    }

    private void OnSingleImageClicked(object sender, TappedRoutedEventArgs e)
    {
        if (SingleImage is { } image)
        {
            _imageViewerService.RequestImageViewer([image]);
        }
    }

    private void OnVideoSurfaceTapped(object sender, TappedRoutedEventArgs e)
    {
        if (e.OriginalSource is Grid &&
            sender is MediaPlayerElement { MediaPlayer: MediaPlayer { PlaybackSession.PlaybackState: MediaPlaybackState state } mp })
        {
            e.Handled = true;
            if (state is MediaPlaybackState.Playing)
            {
                mp.Pause();
            }
            else if (state is MediaPlaybackState.Paused)
            {
                mp.Play();
            }
        }
    }

    private void OnVideoSurfaceKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (sender is MediaPlayerElement { IsFullWindow: true } mpe && e.Key is VirtualKey.Escape)
        {
            e.Handled = true;
            mpe.IsFullWindow = false;
        }
    }

    private async void OnStarterPackClicked(object sender, RoutedEventArgs e)
    {
        string link = Embed?.Record?.StarterPackLink() ?? string.Empty;
        if (!string.IsNullOrEmpty(link) && Uri.TryCreate(link, UriKind.Absolute, out Uri uri))
        {
            try
            {
                await Launcher.LaunchUriAsync(uri);
            }
            catch { }
        }
    }
}
