using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using BlueskyClient.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class EmbeddedPost : UserControl
{
    public static readonly DependencyProperty RecordProperty = DependencyProperty.Register(
        nameof(Record), 
        typeof(FeedRecord), 
        typeof(EmbeddedPost), 
        new PropertyMetadata(null, OnRecordChanged));

    private static void OnRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EmbeddedPost post)
        {
            post.Update();
        }
    }

    public EmbeddedPost()
    {
        this.InitializeComponent();
    }

    public FeedRecord? Record
    {
        get => (FeedRecord?)GetValue(RecordProperty);
        set => SetValue(RecordProperty, value);
    }

    public AuthorViewModel AuthorViewModel { get; } = new();

    public string PostText => Record?.Value?.Text ?? string.Empty;

    public PostEmbed? PostEmbed => Record?.Embeds is [PostEmbed first, ..] ? first : null;

    public bool PostEmbedVisible => PostEmbed is not null;

    public int QuotePostMaxLines => PostEmbedVisible ? 3 : 0;

    private void Update()
    {
        AuthorViewModel.SetAuthor(Record?.Author);
        this.Bindings.Update();
    }
}
