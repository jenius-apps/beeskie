using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(string accessToken)
    {
        var timelineUrl = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.NotificationsPath}";
        HttpRequestMessage message = new(HttpMethod.Get, timelineUrl);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var httpResponse = await _httpClient.SendAsync(message);
            if (httpResponse.IsSuccessStatusCode)
            {
                using Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
                var response = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.FeedResponse);
                return response?.Notifications ?? [];
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }

        return [];
    }

    /// <inheritdoc/>
    public async Task<Result<int>> GetUnreadCountAsync(string accessToken, CancellationToken ct)
    {
        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.UnreadCountPath}";
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
