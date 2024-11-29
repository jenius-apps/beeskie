using Bluesky.NET.ApiClients;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Services;
using FluentResults;
using JeniusApps.Common.Telemetry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueskyClient.Caches;

public class ProfileCache : ICache<Author>
{
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITelemetry _telemetry;
    private readonly ConcurrentDictionary<string, CachedItem<Author>> _cache = new();

    public ProfileCache(
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService,
        ITelemetry telemetry)
    {
        _apiClient = blueskyApiClient;
        _authenticationService = authenticationService;
        _telemetry = telemetry;
    }

    public async Task<Author?> GetItemAsync(string identifier)
    {
        if (_cache.TryGetValue(identifier, out CachedItem<Author> cachedResult) &&
            DateTime.Now < cachedResult.ExpirationTime)
        {
            return cachedResult.Data;
        }

        // At this point, either the data doesn't exist or it's expired.
        // Regardless, get fresh data.

        Result<string> accessTokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (accessTokenResult.IsFailed)
        {
            return null;
        }

        Author? author = null;

        try
        {
            author = await _apiClient.GetAuthorAsync(accessTokenResult.Value, identifier);
        }
        catch (Exception e)
        {
            var dict = new Dictionary<string, string>
            {
                { "method", "GetAuthorAsync" },
                { "message", e.Message },
            };
            _telemetry.TrackError(e, dict);
            _telemetry.TrackEvent(TelemetryConstants.ApiError, dict);
        }

        if (author is null)
        {
            return null;
        }

        var newCachedItem = new CachedItem<Author>
        {
            Data = author,
            ExpirationTime = DateTime.Now.AddHours(UrlConstants.OnlineDataHoursToLive)
        };

        _cache.AddOrUpdate(identifier, newCachedItem, (key, item) => newCachedItem);
        return author;
    }

    public Task<IReadOnlyDictionary<string, Author>> GetItemsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyDictionary<string, Author>> GetItemsAsync(IReadOnlyList<string> ids)
    {
        throw new NotImplementedException();
    }
}
