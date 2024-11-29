namespace Bluesky.NET.Models;

/// <summary>
/// Metadata describing a feed object such as 
/// Following and Discover feeds.
/// </summary>
public class FeedMetaData
{
    /// <summary>
    /// A short string uniquely identifying the feed object.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// One of 'feed', 'list', or 'timeline'.
    /// </summary>
    /// <remarks>
    /// Source: https://docs.bsky.app/docs/api/app-bsky-actor-get-preferences.
    /// </remarks>
    public string? Type { get; init; }

    /// <summary>
    /// The at:// uri for the feed.
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// Specifies if the user has pinned the feed.
    /// </summary>
    public bool Pinned { get; init; }
}
