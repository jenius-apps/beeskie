namespace BlueskyClient.Models;

/// <summary>
/// Class used as argument parameters when navigating
/// in PostPage and PostThreadControl.
/// </summary>
public class PostThreadArgs
{
    /// <summary>
    /// The AT URI for the post thread.
    /// </summary>
    public required string AtUri { get; init; }
}
