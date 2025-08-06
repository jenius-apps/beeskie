using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using FluentResults;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

/// <summary>
/// Class for retrieving the entire thread of posts.
/// </summary>
public sealed class PostThreadService : IPostThreadService
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IBlueskyApiClient _blueskyApiClient;

    public PostThreadService(
        IAuthenticationService authenticationService,
        IBlueskyApiClient blueskyApiClient)
    {
        _authenticationService = authenticationService;
        _blueskyApiClient = blueskyApiClient;
    }

    /// <inheritdoc/>
    public async Task<PostThread?> GetThreadAsync(string atUri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (atUri is not { Length: > 0 })
        {
            return null;
        }

        Result<string> tokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (tokenResult.IsFailed)
        {
            return null;
        }

        Result<PostThreadResponse?> threadResponse = await _blueskyApiClient.GetPostThreadAsync(
            tokenResult.Value,
            atUri,
            cancellationToken);

        return threadResponse.IsSuccess && threadResponse.Value?.Thread is { } postThread
            ? postThread
            : null;
    }
}
