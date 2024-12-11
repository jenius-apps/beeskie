using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using FluentResults;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public sealed class DiscoverService : IDiscoverService
{
    private readonly IBlueskyApiClient _apiClient;
    private readonly IAuthenticationService _authenticationService;

    public DiscoverService(
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService)
    {
        _apiClient = blueskyApiClient;
        _authenticationService = authenticationService;
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<Author> Authors, string? cursor)> GetSuggestedPeopleAsync(
        CancellationToken ct,
        int count = 10,
        string? cursor = null)
    {
        ct.ThrowIfCancellationRequested();

        var tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (tokenResult.IsFailed)
        {
            return ([], null);
        }

        Result<FeedResponse> result = await _apiClient.GetSuggestedPeopleAsync(tokenResult.Value, ct, count, cursor);

        return result.IsSuccess && result.Value.Actors is IReadOnlyList<Author> authors
            ? (authors, result.Value.Cursor)
            : ([], null);
    }
}
