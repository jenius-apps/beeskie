using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using System;
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
    /// Raised when a recent search was added.
    /// </summary>
    event EventHandler<string>? RecentSearchAdded;

    /// <summary>
    /// Gets list of recent searches orderd by most recent to oldest.
    /// </summary>
    /// <returns>List of recent searches.</returns>
    IReadOnlyList<string> GetRecentSearches();

    /// <summary>
    /// Deletes the given search from the user's history.
    /// </summary>
    /// <param name="query">The query to delete.</param>
    void DeleteRecentSearch(string query);

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