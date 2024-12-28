using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    public async Task<IReadOnlyList<FeedPost>> GetPostsAsync(string accessToken, IReadOnlyList<string> atUriList)
    {
        if (atUriList.Count == 0)
        {
            return [];
        }

        var timelineUrl = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.PostsPath}?uris={string.Join(",", atUriList)}";
        HttpRequestMessage message = new(HttpMethod.Get, timelineUrl);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var httpResponse = await _httpClient.SendAsync(message);
            if (httpResponse.IsSuccessStatusCode)
            {
                using Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
                var response = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.FeedResponse);
                return response?.Posts ?? [];
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
    public async Task<CreateRecordResponse?> SubmitPostAsync(string accessToken, string handle, SubmissionRecord record, RecordType recordType)
    {
        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.CreateRecordPath}";

        CreateRecordBody body = new()
        {
            Repo = handle,
            Record = record,
            Collection = recordType.ToStringType()
        };

        HttpRequestMessage message = new(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, ModelSerializerContext.CaseInsensitive.CreateRecordBody), Encoding.UTF8, "application/json"),
        };

        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await _httpClient.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                var responseRecord = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.CreateRecordResponse);
                return responseRecord;
            }
            else
            {
                var errorText = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(errorText);
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
    public async Task SubmitPostUndoAsync(string accessToken, string handle, string rkey, RecordType recordType, CancellationToken cancellationToken)
    {
        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.DeleteRecordPath}";

        DeleteRecordBody body = new()
        {
            Repo = handle,
            Rkey = rkey,
            Collection = recordType.ToStringType()
        };

        HttpRequestMessage message = new(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, ModelSerializerContext.CaseInsensitive.DeleteRecordBody), Encoding.UTF8, "application/json"),
        };

        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        _ = await SendMessageAsync(accessToken, message, cancellationToken);
    }
}
