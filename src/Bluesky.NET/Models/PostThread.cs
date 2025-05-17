namespace Bluesky.NET.Models;

public class PostThreadResponse
{
    public PostThread? Thread { get; init; }
}

public class PostThread : TypedItem
{
    public FeedPost? Post { get; init; }

    public PostThread[] Replies { get; init; } = [];
}
