using System.Text.Json.Serialization;
using Bluesky.NET.Models.JsonConverters;

namespace Bluesky.NET.Models;

public class FeedResponse
{
    public string? Cursor { get; init; }

    public Author[]? Actors { get; init; }

    public FeedItem[]? Feed { get; init; }

    public FeedGenerator[]? Feeds { get; init; }

    public Notification[]? Notifications { get; init; }

    public FeedPost[]? Posts { get; init; }

    public Blob? Blob { get; init; }

    [JsonConverter(typeof(ArrayConverter<PreferenceItem, PreferenceItemConverter>))]
    public PreferenceItem[]? Preferences { get; init; }
}
