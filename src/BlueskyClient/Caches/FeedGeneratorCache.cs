using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Services;
using FluentResults;

namespace BlueskyClient.Caches;

public class FeedGeneratorCache : ICache<FeedGenerator>
{
    private static readonly IReadOnlyDictionary<string, FeedGenerator> EmptyDictionary = new Dictionary<string, FeedGenerator>();
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly ConcurrentDictionary<string, FeedGenerator> _cache = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public FeedGeneratorCache(
        IBlueskyApiClient apiClient,
        IAuthenticationService authenticationService)
    {
        _apiClient = apiClient;
        _authenticationService = authenticationService;
    }

    public Task<FeedGenerator?> GetItemAsync(string id, CancellationToken ct)
    {
        throw new NotSupportedException();
    }

    public Task<IReadOnlyDictionary<string, FeedGenerator>> GetItemsAsync(CancellationToken ct)
    {
        throw new NotSupportedException();
    }

    public async Task<IReadOnlyDictionary<string, FeedGenerator>> GetItemsAsync(
        IReadOnlyList<string> atUris,
        CancellationToken ct)
    {
        Dictionary<string, FeedGenerator> result = [];
        var missingUris = TryPopulate(result, atUris, _cache);
        if (missingUris.Count == 0)
        {
            return result;
        }

        await _lock.WaitAsync(ct);

        missingUris = TryPopulate(result, atUris, _cache);
        if (missingUris.Count == 0)
        {
            return result;
        }

        var tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (tokenResult.IsFailed)
        {
            _lock.Release();
            return EmptyDictionary;
        }

        Result<IReadOnlyList<FeedGenerator>> response = await _apiClient.GetFeedGeneratorsAsync(
            tokenResult.Value,
            missingUris,
            ct);

        if (response.IsFailed)
        {
            _lock.Release();
            return EmptyDictionary;
        }

        foreach (var feed in response.Value)
        {
            if (feed is { Uri: { Length: > 0 } uri })
            {
                _cache[uri] = feed;
                result.Add(uri, feed);
            }
        }

        _lock.Release();
        return result;
    }

    private static IReadOnlyList<string> TryPopulate(
        Dictionary<string, FeedGenerator> result,
        IReadOnlyList<string> keys,
        ConcurrentDictionary<string, FeedGenerator> cache)
    {
        List<string> missingKeys = [];

        foreach (var key in keys)
        {
            if (result.ContainsKey(key))
            {
                continue;
            }
            else if (cache.TryGetValue(key, out FeedGenerator storedFeed))
            {
                result.Add(key, storedFeed);
            }
            else
            {
                missingKeys.Add(key);
            }
        }

        return missingKeys;
    }
}
