using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;

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

        var feedParameters = atUris
            .Where(x => x.StartsWith("at://"))
            .Select(x => $"feeds={x}");

        string combined = string.Join("&", feedParameters);

        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.FeedGeneratorsPath}?{combined}";
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
