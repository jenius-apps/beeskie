using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<FeedGenerator>>> GetFeedGeneratorsAsync(
        string accessToken,
        IReadOnlyList<string> atUris,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.FeedGeneratorsPath}?feeds={string.Join(",", atUris)}";
        HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        Result<FeedResponse> response = await SendMessageAsync(
            message,
            ModelSerializerContext.CaseInsensitive.FeedResponse,
            ct);

        return response.IsSuccess && response.Value.Feeds is IReadOnlyList<FeedGenerator> feeds
            ? Result.Ok(feeds)
            : Result.Fail<IReadOnlyList<FeedGenerator>>(response.Errors);
    }
}
