namespace Bluesky.NET.Models;

public class PreferenceItem : TypedItem
{
    public const string PreferenceSavedFeedsTypeKey = "app.bsky.actor.defs#savedFeedsPrefV2";

    public FeedMetaData[]? Items { get; init; }
}