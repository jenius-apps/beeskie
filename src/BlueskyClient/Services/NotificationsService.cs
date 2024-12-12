using Bluesky.NET.ApiClients;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.ViewModels;
using FluentResults;
using JeniusApps.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public class NotificationsService : INotificationsService
{
    private readonly IBlueskyApiClient _blueskyApiClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITelemetry _telemetry;

    public NotificationsService(
        IBlueskyApiClient blueskyApiClient,
        IAuthenticationService authenticationService,
        ITelemetry telemetry)
    {
        _blueskyApiClient = blueskyApiClient;
        _authenticationService = authenticationService;
        _telemetry = telemetry;
    }

    /// <inheritdoc/>
    public async Task<int> GetUnreadCountAsync(CancellationToken ct)
    {
        Result<string> accessTokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (accessTokenResult.IsFailed)
        {
            return 0;
        }

        var result = await _blueskyApiClient.GetUnreadCountAsync(accessTokenResult.Value, ct);
        return result.IsSuccess
            ? result.Value
            : 0;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync()
    {
        Result<string> accessTokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (accessTokenResult.IsFailed)
        {
            return [];
        }

        try
        {
            return await _blueskyApiClient.GetNotificationsAsync(accessTokenResult.Value);
        }
        catch (Exception e)
        {
            var dict = new Dictionary<string, string>
            {
                { "method", "GetNotificationsAsync" },
                { "message", e.Message },
            };
            _telemetry.TrackError(e, dict);
            _telemetry.TrackEvent(TelemetryConstants.ApiError, dict);
            return [];
        }
    }

    public async Task HydrateAsync(NotificationViewModel notification)
    {
        Result<string> accessTokenResult = await _authenticationService.TryGetFreshTokenAsync();
        if (accessTokenResult.IsFailed)
        {
            return;
        }

        if (notification.Reason is ReasonConstants.Like or ReasonConstants.Repost &&
            notification.Notification.ReasonSubject is string { Length: > 0 } subjectUri)
        {
            IReadOnlyList<FeedPost> subjectPosts;

            try
            {
                subjectPosts = await _blueskyApiClient.GetPostsAsync(accessTokenResult.Value, [subjectUri]);
            }
            catch (Exception e)
            {
                var dict = new Dictionary<string, string>
                {
                    { "method", "GetPostsAsync" },
                    { "message", e.Message },
                };
                _telemetry.TrackError(e, dict);
                _telemetry.TrackEvent(TelemetryConstants.ApiError, dict);

                subjectPosts = [];
            }

            notification.SubjectPost = subjectPosts.Count > 0
                ? subjectPosts[0]
                : null;
        }
    }
}
