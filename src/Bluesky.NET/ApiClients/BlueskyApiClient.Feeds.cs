using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    /// <inheritdoc/>
    public async Task<Result<FeedResponse>> GetFeedAsync(
        string accessToken,
        string atUri,
        CancellationToken ct,
        string? cursor = null)
    {
        ct.ThrowIfCancellationRequested();

        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.FeedPath}?feed={atUri}";
        if (cursor is { Length: > 0 } cursorParameter)
        {
            url += $"&cursor={cursorParameter}";
        }

        HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        Result<FeedResponse> response = await SendMessageAsync(
            message,
            ModelSerializerContext.CaseInsensitive.FeedResponse,
            ct);

        return response.IsSuccess && response.Value is FeedResponse feedResponse
            ? Result.Ok(feedResponse)
            : Result.Fail<FeedResponse>(response.Errors);
    }

    public async Task<FeedResponse> GetTimelineAsync(string accesstoken, string? cursor = null)
    {
        var timelineUrl = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.TimelinePath}";
        if (cursor is { Length: > 0 } cursorParameter)
        {
            timelineUrl += $"?cursor={cursorParameter}";
        }

        HttpRequestMessage message = new(HttpMethod.Get, timelineUrl);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
        try
        {
            var httpResponse = await _httpClient.SendAsync(message);
            if (httpResponse.IsSuccessStatusCode)
            {
                using Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
                FeedResponse? response = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.FeedResponse);
                return response ?? new();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }

        return new();
    }

    public async Task<IReadOnlyList<FeedItem>> GetAuthorFeedAsync(string accesstoken, string handle)
    {
        var feedUrl = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.AuthorFeedPath}?actor={handle}";
        HttpRequestMessage message = new(HttpMethod.Get, feedUrl);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        try
        {
            var httpResponse = await _httpClient.SendAsync(message);
            if (httpResponse.IsSuccessStatusCode)
            {
                using Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
                FeedResponse? response = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.FeedResponse);
                return response?.Feed ?? [];
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }

        return [];
    }
}
