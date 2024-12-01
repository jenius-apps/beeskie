using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class FeedGenerator
{
    [JsonIgnore]
    public virtual bool IsTimeline { get; }

    public string? Uri { get; init; }

    public string? Cid { get; init; }

    public string? Did { get; init; }

    public Author? Creator { get; init; }

    public string? DisplayName { get; init; }

    public string? Description { get; init; }

    public string? Avatar { get; init; }

    public double LikeCount { get; init; }
}

public class FeedGeneratorWrapper
{
    public bool IsValid { get; init; }

    public bool IsOnline { get; init; }

    public FeedGenerator? View { get; init; }
}

public class TimelineFeedGenerator : FeedGenerator
{
    [JsonIgnore]
    public override bool IsTimeline => true;
}
