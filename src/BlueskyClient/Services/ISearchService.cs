using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

/// <summary>
/// Interface for performing a search against Bluesky's APIs.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Performs a search based on the given query.
    /// </summary>
    /// <param name="query">The user's query.</param>
    /// <param name="ct">A cancellation token. </param>
    /// <param name="cursor">A string used to fetch the next page.</param>
    /// <param name="options">Seach options to use with the query.</param>
    /// <returns>List of post results.</returns>
    Task<(IReadOnlyList<FeedPost> Posts, string? Cursor)> SearchPostsAsync(
        string query,
        CancellationToken ct,
        string? cursor = null,
        SearchOptions? options = null);
}