using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    /// <inheritdoc/>
    public async Task<Result<FeedResponse>> SearchActorsAsync(
        string accessToken,
        string query, 
        CancellationToken ct,
        string? cursor = null)
    {
        // Ref: https://docs.bsky.app/docs/api/app-bsky-actor-search-actors
        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.SearchActorsPath}?q={HttpUtility.UrlEncode(query)}";

        if (cursor is not null)
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

    public async Task<Result<(IReadOnlyList<FeedPost> Posts, string? Cursor)>> SearchPostsAsync(
        string accessToken,
        string query,
        CancellationToken ct,
        string? cursor = null,
        SearchOptions? options = null)
    {
        // Ref: https://docs.bsky.app/docs/api/app-bsky-feed-search-posts
        options ??= new();

        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.SearchPostsPath}?q={HttpUtility.UrlEncode(query)}";

        if (cursor is not null)
        {
            url += $"&cursor={cursor}";
        }

        url += $"&{OptionsToQueryString(options)}";

        HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        Result<FeedResponse> response = await SendMessageAsync(
            message,
            ModelSerializerContext.CaseInsensitive.FeedResponse,
            ct);

        return response.IsSuccess && response.Value.Posts is IReadOnlyList<FeedPost> posts
            ? Result.Ok((posts, response.Value.Cursor))
            : Result.Fail<(IReadOnlyList<FeedPost> Posts, string? Cursor)>(response.Errors);
    }

    private static string OptionsToQueryString(SearchOptions options)
    {
        List<string> queryParamPairs = [$"sort={options.Sort}"];

        // TODO implement the other filters

        return string.Join("&", queryParamPairs);
    }
}

public class SearchOptions
{
    public string Sort { get; init; } = SearchConstants.SortTop;

    public DateTime? Since { get; init; }

    public DateTime? Until { get; init; }

    public string? IdentifierMentioned { get; init; }

    public string? Language { get; init; }

    public string? Domain { get; init; }

    public string? Url { get; init; }
}
