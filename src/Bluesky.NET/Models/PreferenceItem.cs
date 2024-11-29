namespace Bluesky.NET.Models;

public class PreferenceItem : TypedItem
{
    public const string SavedFeedsKey = "app.bsky.actor.defs#savedFeedsPrefV2";
}

public class PreferenceItemSavedFeeds : PreferenceItem
{
    public FeedMetaData[]? Items { get; init; }
}