using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(string accessToken)
    {
        var url = $"{_baseUrl}/{UrlConstants.NotificationsPath}";
        HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var result = await SendMessageAsync(message, ModelSerializerContext.CaseInsensitive.FeedResponse, default);

        if (result.IsSuccess)
        {
            await UpdateSeenAsync(accessToken, default);
        }

        return result.IsSuccess && result.Value.Notifications is IReadOnlyList<Notification> list
            ? list
            : [];
    }

    /// <inheritdoc/>
    public async Task<Result> UpdateSeenAsync(string accessToken, CancellationToken ct)
    {
        var url = $"{_baseUrl}/{UrlConstants.UpdateSeenPath}";
        UpdateSeenBody body = new()
        {
            SeenAt = DateTime.Now.ToUniversalTime().ToString("O")
        };

        HttpRequestMessage message = new(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, ModelSerializerContext.CaseInsensitive.UpdateSeenBody), Encoding.UTF8, "application/json"),
        };

        return await SendMessageAsync(accessToken, message, ct);
    }

    /// <inheritdoc/>
    public async Task<Result<int>> GetUnreadCountAsync(string accessToken, CancellationToken ct)
    {
        var url = $"{_baseUrl}/{UrlConstants.UnreadCountPath}";
        HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var result = await SendMessageAsync(
            message,
            ModelSerializerContext.CaseInsensitive.FeedResponse,
            ct);

        return result.IsSuccess
            ? Result.Ok(result.Value.Count ?? 0)
            : Result.Fail<int>(result.Errors);
    }
}
