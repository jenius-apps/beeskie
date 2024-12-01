using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface ITimelineService
{
    /// <summary>
    /// Retrieves the feed items for the given Feed based on its At URI.
    /// </summary>
    /// <param name="feedAtUri">The At URI of the feed to fetch.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <param name="cursor">A bluesky-provided string to support pagination.</param>
    /// <returns>List of feed items and a cursor string used for pagination.</returns>
    Task<(IReadOnlyList<FeedItem> Items, string? Cursor)> GetFeedItemsAsync(
        string feedAtUri,
        CancellationToken ct,
        string? cursor = null);
    Task<(IReadOnlyList<FeedItem> Items, string? Cursor)> GetTimelineAsync(CancellationToken ct, string? cursor = null);
}