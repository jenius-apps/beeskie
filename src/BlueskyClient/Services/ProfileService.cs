﻿using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Caches;
using BlueskyClient.Constants;
using FluentResults;
using JeniusApps.Common.Settings;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public class ProfileService : IProfileService
{
    private readonly ICache<Author> _profileCache;
    private readonly IUserSettings _userSettings;
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;

    public ProfileService(
        ICache<Author> profileCache,
        IUserSettings userSettings,
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService)
    {
        _profileCache = profileCache;
        _userSettings = userSettings;
        _apiClient = blueskyApiClient;
        _authenticationService = authenticationService;
    }

    /// <inheritdoc/>
    public async Task<Author?> GetCurrentUserAsync()
    {
        string? identifier = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey);
        if (identifier is null)
        {
            return null;
        }

        return await _profileCache.GetItemAsync(identifier, default);
    }

    /// <inheritdoc/>
    public async Task<string?> GetDIDAsync(string? atIdenfitifer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await GetFullAuthorProfileAsync(atIdenfitifer, cancellationToken) is { Did: string did }
            ? did
            : null;
    }

    /// <inheritdoc/>
    public async Task<Author?> GetFullAuthorProfileAsync(string? identifier, CancellationToken cancellationToken)
    {
        if (identifier is not { Length: > 0 })
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        return await _profileCache.GetItemAsync(identifier, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FeedItem>> GetProfileFeedAsync(string handle)
    {
        if (handle is not { Length: > 0 })
        {
            return [];
        }

        Result<string> tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (tokenResult.IsFailed)
        {
            return [];
        }

        return await _apiClient.GetAuthorFeedAsync(tokenResult.Value, handle);
    }

    /// <inheritdoc/>
    public async Task<bool> FollowActorAsync(string subjectDid, CancellationToken ct)
    {
        if (subjectDid is not { Length: > 0 })
        {
            return false;
        }

        Author? currentUser = await GetCurrentUserAsync();
        var tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (tokenResult.IsFailed || currentUser?.Handle is not { Length: > 0 } handle)
        {
            return false;
        }

        return await _apiClient.FollowActorAsync(
            tokenResult.Value,
            handle,
            subjectDid,
            ct);
    }
}
