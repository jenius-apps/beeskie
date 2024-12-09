using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Models;
using FluentResults;
using JeniusApps.Common.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public class SearchService : ISearchService
{
    private const int RecentSearchMaxCount = 5;
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserSettings _userSettings;

    public SearchService(
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService,
        IUserSettings userSettings)
    {
        _apiClient = blueskyApiClient;
        _authenticationService = authenticationService;
        _userSettings = userSettings;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetRecentSearches()
    {
        return _userSettings.GetAndDeserialize(
            UserSettingsConstants.RecentSearchesKey,
            LocalSerializerContext.CaseInsensitive.StringArray) ?? [];
    }

    /// <inheritdoc/>
    public void DeleteRecentSearch(string query)
    {
        if (query is not { Length: > 0 })
        {
            return;
        }

        var recentSearches = GetRecentSearches();
        if (!recentSearches.Contains(query))
        {
            return;
        }

        var newSearches = recentSearches.Where(x => x != query).ToArray();
        _userSettings.SetAndSerialize(
            UserSettingsConstants.RecentSearchesKey,
            [.. newSearches],
            LocalSerializerContext.CaseInsensitive.StringArray);
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

        if (searchResult.IsSuccess)
        {
            AddRecentSearch(query);
        }

        return searchResult.IsSuccess
            ? searchResult.Value
            : ([], null);
    }

    private void AddRecentSearch(string query)
    {
        IReadOnlyList<string> searches = GetRecentSearches();

        if (searches.Contains(query))
        {
            return;
        }

        List<string> newSearches = [query];

        if (searches.Count >= RecentSearchMaxCount)
        {
            newSearches.AddRange(searches.Take(RecentSearchMaxCount - 1));
        }
        else
        {
            newSearches.AddRange(searches);
        }

        _userSettings.SetAndSerialize(
            UserSettingsConstants.RecentSearchesKey,
            [.. newSearches],
            LocalSerializerContext.CaseInsensitive.StringArray);
    }
}
