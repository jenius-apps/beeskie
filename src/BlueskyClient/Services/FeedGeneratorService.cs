using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluesky.NET.ApiClients;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Caches;

namespace BlueskyClient.Services;

public class FeedGeneratorService : IFeedGeneratorService
{
    private readonly TimelineFeedGenerator _timelineFeed = new();
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly ICache<FeedGenerator> _feedCache;

    public FeedGeneratorService(
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService,
        ICache<FeedGenerator> feedCache)
    {
        _apiClient = blueskyApiClient;
        _authenticationService = authenticationService;
        _feedCache = feedCache;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FeedGenerator>> GetSavedFeedsAsync(bool pinnedFeedsOnly, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (tokenResult.IsFailed)
        {
            return [];
        }

        ct.ThrowIfCancellationRequested();
        var preferencesResult = await _apiClient.GetPreferencesAsync(
            tokenResult.Value,
            ct);

        if (preferencesResult.IsFailed)
        {
            return [];
        }

        PreferenceItemSavedFeeds? savedFeeds = preferencesResult.Value
            .Where(x => x is PreferenceItemSavedFeeds)
            .Cast<PreferenceItemSavedFeeds>()
            .FirstOrDefault();

        if (savedFeeds.Items is not { Length: > 0 } preferenceFeedInfoList)
        {
            return [];
        }

        List<string> feedUris = preferenceFeedInfoList
            .Where(x => x.Type == PreferenceFeedTypes.Feed)
            .Where(x => !pinnedFeedsOnly || x.Pinned)
            .Select(x => x.Value ?? string.Empty)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        IReadOnlyDictionary<string, FeedGenerator> feeds = await _feedCache.GetItemsAsync(feedUris, ct);

        ct.ThrowIfCancellationRequested();
        List<FeedGenerator> result = [];
        foreach (var feedInfo in preferenceFeedInfoList)
        {
            if (feedInfo is { Type: PreferenceFeedTypes.Timeline })
            {
                result.Add(_timelineFeed);
            }
            else if (feedInfo is { Type: PreferenceFeedTypes.Feed, Value: string atUri } &&
                feeds.TryGetValue(atUri, out var feed))
            {
                result.Add(feed);
            }
        }

        return result;
    }
}
