using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class PostEmbed
{
    [JsonPropertyName("$type")]
    public string Type { get; init; } = string.Empty;

    public ImageEmbed[]? Images { get; init; }

    public ExternalEmbed? External { get; init; }

    public FeedRecord? Record { get; init; }

    public string? Playlist { get; init; }

    public string? Thumbnail { get; init; }

    public AspectRatio? AspectRatio { get; init; }
}
