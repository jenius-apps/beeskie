namespace Bluesky.NET.Models;

public class FeedGenerator
{
    public bool IsValid { get; init; }

    public bool IsOnline { get; init; }

    public FeedGeneratorView? View { get; init; }
}

public class FeedGeneratorView
{
    public string? Uri { get; init; }

    public string? Cid { get; init; }

    public string? Did { get; init; }

    public Author? Creator { get; init; }

    public string? DisplayName { get; init; }

    public string? Description { get; init; }

    public string? Avatar { get; init; }

    public double LikeCount { get; init; }
}
