namespace Bluesky.NET.Constants;

public sealed class UrlConstants
{
    public const int OnlineDataHoursToLive = 2; // 2 hour cache for some online data.

    public const string BlueskyBaseUrl = "https://bsky.social";
    public const string AuthPath = "xrpc/com.atproto.server.createSession";
    public const string RefreshAuthPath = "xrpc/com.atproto.server.refreshSession";
    public const string TimelinePath = "xrpc/app.bsky.feed.getTimeline";
    public const string FeedPath = "xrpc/app.bsky.feed.getFeed";
    public const string ProfilePath = "xrpc/app.bsky.actor.getProfile";
    public const string NotificationsPath = "xrpc/app.bsky.notification.listNotifications";
    public const string PostsPath = "xrpc/app.bsky.feed.getPosts";
    public const string CreateRecordPath = "xrpc/com.atproto.repo.createRecord";
    public const string DeleteRecordPath = "xrpc/com.atproto.repo.deleteRecord";
    public const string AuthorFeedPath = "xrpc/app.bsky.feed.getAuthorFeed";
    public const string UploadBlobPath = "xrpc/com.atproto.repo.uploadBlob";
    public const string PreferencesPath = "xrpc/app.bsky.actor.getPreferences";
    public const string FeedGeneratorPath = "xrpc/app.bsky.feed.getFeedGenerator";
    public const string FeedGeneratorsPath = "xrpc/app.bsky.feed.getFeedGenerators";
    public const string SearchPostsPath = "xrpc/app.bsky.feed.searchPosts";
    public const string SearchActorsPath = "xrpc/app.bsky.actor.searchActors";
    public const string SearchFeedsPath = "xrpc/app.bsky.unspecced.getPopularFeedGenerators";
    public const string SuggestedPeoplePath = "xrpc/app.bsky.actor.getSuggestions";
    public const string SuggestedFeedsPath = "xrpc/app.bsky.feed.getSuggestedFeeds";
    public const string UnreadCountPath = "xrpc/app.bsky.notification.getUnreadCount";
    public const string UpdateSeenPath = "xrpc/app.bsky.notification.updateSeen";
}
