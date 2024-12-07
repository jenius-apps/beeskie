using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using FluentResults;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public class SearchService : ISearchService
{
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;

    public SearchService(
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService)
    {
        _apiClient = blueskyApiClient;
        _authenticationService = authenticationService;
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<FeedPost> Posts, string? Cursor)> SearchPostsAsync(
        string query,
        CancellationToken ct,
        string? cursor = null,
        SearchOptions? options = null)
    {
        query = query.Trim();
        if (string.IsNullOrEmpty(query))
        {
            return ([], null);
        }

        var tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (tokenResult.IsFailed)
        {
            return ([], null);
        }

        Result<(IReadOnlyList<FeedPost> Posts, string? Cursor)> searchResult = await _apiClient.SearchPostsAsync(
            tokenResult.Value,
            query,
            ct,
            cursor,
            options);

        return searchResult.IsSuccess
            ? searchResult.Value
            : ([], null);
    }
}
