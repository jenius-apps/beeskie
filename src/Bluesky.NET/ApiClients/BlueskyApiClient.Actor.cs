using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;
using System;
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
    public async Task<Author?> GetAuthorAsync(string accessToken, string identifier)
    {
        var timelineUrl = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.ProfilePath}?actor={identifier}";
        HttpRequestMessage message = new(HttpMethod.Get, timelineUrl);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var httpResponse = await _httpClient.SendAsync(message);
            if (httpResponse.IsSuccessStatusCode)
            {
                using Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
                Author? response = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.Author);
                return response ?? null;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<Result<FeedResponse>> GetSuggestedPeopleAsync(
        string accessToken,
        CancellationToken ct,
        int count = 10,
        string? cursor = null)
    {
        if (count < 1)
        {
            count = 1;
        }
        else if (count > 100)
        {
            count = 100;
        }

        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.SuggestedPeoplePath}?limit={count}";
        if (cursor is { Length: > 0 })
        {
            url += $"&cursor={cursor}";
        }

        HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await SendMessageAsync(
            message,
            ModelSerializerContext.CaseInsensitive.FeedResponse,
            ct);
    }
}
