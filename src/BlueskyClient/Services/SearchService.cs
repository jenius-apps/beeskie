using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Models;
using FluentResults;
using JeniusApps.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public class SearchService : ISearchService
{
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserSettings _userSettings;
    private readonly IModerationService _moderationService;

    /// <inheritdoc/>
    public event EventHandler<string>? RecentSearchAdded;

    public SearchService(
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService,
        IUserSettings userSettings,
        IModerationService moderationService)
    {
        _apiClient = blueskyApiClient;
        _authenticationService = authenticationService;
        _userSettings = userSettings;
        _moderationService = moderationService;
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

        IEnumerable<FeedPost> moderatedItems = _moderationService.ModerateItems(searchResult.Value.Posts);

        return searchResult.IsSuccess
            ? ([.. moderatedItems], searchResult.Value.Cursor)
            : ([], null);
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<FeedGenerator> Feeds, string? Cursor)> SearchFeedsAsync(
        string query,
        CancellationToken ct,
        string? cursor = null)
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

        Result<FeedResponse> searchResult = await _apiClient.SearchFeedsAsync(
            tokenResult.Value,
            query,
            ct,
            cursor);

        if (searchResult.IsSuccess)
        {
            AddRecentSearch(query);
        }

        return searchResult.IsSuccess && searchResult.Value.Feeds is IReadOnlyList<FeedGenerator> feeds
            ? (feeds, searchResult.Value.Cursor)
            : ([], null);
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<Author> Actors, string? Cursor)> SearchActorsAsync(
        string query,
        CancellationToken ct,
        string? cursor = null)
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

        Result<FeedResponse> searchResult = await _apiClient.SearchActorsAsync(
            tokenResult.Value,
            query,
            ct,
            cursor);

        if (searchResult.IsSuccess)
        {
            AddRecentSearch(query);
        }

        return searchResult.IsSuccess && searchResult.Value.Actors is IReadOnlyList<Author> authors
            ? (authors, searchResult.Value.Cursor)
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

        if (searches.Count >= SearchConstants.RecentSearchMaxCount)
        {
            newSearches.AddRange(searches.Take(SearchConstants.RecentSearchMaxCount - 1));
        }
        else
        {
            newSearches.AddRange(searches);
        }

        _userSettings.SetAndSerialize(
            UserSettingsConstants.RecentSearchesKey,
            [.. newSearches],
            LocalSerializerContext.CaseInsensitive.StringArray);

        RecentSearchAdded?.Invoke(this, query);
    }
}
