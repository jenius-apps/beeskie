using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bluesky.NET.ApiClients;

public interface IBlueskyApiClient
{
    /// <summary>
    /// Retrieves authenticated tokens that can be used
    /// for other API calls that require auth.
    /// </summary>
    /// <param name="userHandle">The user's handle or email address or DID.</param>
    /// <param name="appPassword">An app password provided by the user.</param>
    /// <returns>An <see cref="AuthResponse"/>.</returns>
    Task<Result<AuthResponse>> AuthenticateAsync(string identifier, string appPassword);
    Task<FeedResponse> GetTimelineAsync(string accesstoken, string? cursor = null);
    Task<Result<AuthResponse>> RefreshAsync(string refreshToken);

    Task<Author?> GetAuthorAsync(string accessToken, string identifier);

    Task<IReadOnlyList<Notification>> GetNotificationsAsync(string accessToken);
    Task<IReadOnlyList<FeedPost>> GetPostsAsync(string accessToken, IReadOnlyList<string> atUriList);
    Task<CreateRecordResponse?> SubmitPostAsync(string accessToken, string handle, SubmissionRecord record, RecordType recordType);
    Task<IReadOnlyList<FeedItem>> GetAuthorFeedAsync(string accesstoken, string handle);
    Task<Blob?> UploadBlobAsync(string accessToken, byte[] blob, string mimeType);

    /// <summary>
    /// Retrieves the list of user's preferences.
    /// </summary>
    /// <param name="accessToken">Access token for the logged in user.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>List of the user's preferences.</returns>
    Task<Result<IReadOnlyList<PreferenceItem>>> GetPreferencesAsync(
        string accessToken,
        CancellationToken ct);

    /// <summary>
    /// Retrieves list of feed generator data.
    /// </summary>
    /// <param name="accessToken">Access token for the logged in user.</param>
    /// <param name="atUris">List of at:// URIs that represent different feed generators.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>List of feed generator data.</returns>
    Task<Result<IReadOnlyList<FeedGenerator>>> GetFeedGeneratorsAsync(
        string accessToken,
        IReadOnlyList<string> atUris,
        CancellationToken ct);

    /// <summary>
    /// Retrieves the posts for the given feed.
    /// </summary>
    /// <param name="accessToken">Access token for the logged in user.</param>
    /// <param name="atUri">The at:// URI that represents a feed.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <param name="cursor">A bluesky-provided string to help with pagination.</param>
    /// <returns>The items for the feed.</returns>
    Task<Result<FeedResponse>> GetFeedAsync(
        string accessToken,
        string atUri,
        CancellationToken ct,
        string? cursor = null);

    /// <summary>
    /// Performs posts search.
    /// </summary>
    /// <param name="accessToken">Access token for the logged in user.</param>
    /// <param name="query">The user-provided search query to use.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <param name="cursor">A bluesky-provided string to help with pagination.</param>
    /// <param name="options">A <see cref="SearchOptions"/> object with filter parameters.</param>
    /// <returns>List of posts returned by the search service and a cursor.</returns>
    Task<Result<(IReadOnlyList<FeedPost> Posts, string? Cursor)>> SearchPostsAsync(
        string accessToken,
        string query,
        CancellationToken ct,
        string? cursor = null,
        SearchOptions? options = null);
}