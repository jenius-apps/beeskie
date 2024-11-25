using System;
using System.Collections.Generic;
using System.Text;

namespace Bluesky.NET.Constants;

public sealed class UrlConstants
{
    public const int OnlineDataHoursToLive = 2; // 2 hour cache for some online data.

    public const string BlueskyBaseUrl = "https://bsky.social";
    public const string AuthPath = "xrpc/com.atproto.server.createSession";
    public const string RefreshAuthPath = "xrpc/com.atproto.server.refreshSession";
    public const string TimelinePath = "xrpc/app.bsky.feed.getTimeline";
    public const string ProfilePath = "xrpc/app.bsky.actor.getProfile";
    public const string NotificationsPath = "xrpc/app.bsky.notification.listNotifications";
    public const string PostsPath = "xrpc/app.bsky.feed.getPosts";
    public const string CreateRecordPath = "xrpc/com.atproto.repo.createRecord";
    public const string AuthorFeedPath = "xrpc/app.bsky.feed.getAuthorFeed";
    public const string UploadBlobPath = "xrpc/com.atproto.repo.uploadBlob";
}
