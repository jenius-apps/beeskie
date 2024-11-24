namespace Bluesky.NET.Models;

public class FeedResponse
{
    public string? Cursor { get; init; }

    public FeedItem[]? Feed { get; init; }

    public Notification[]? Notifications { get; init; }

    public FeedPost[]? Posts { get; init; }
}
