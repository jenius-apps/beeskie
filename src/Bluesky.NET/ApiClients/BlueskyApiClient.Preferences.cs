using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using System.Text.Json;
using FluentResults;
using System.Threading.Tasks;
using System.Threading;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<PreferenceItem>>> GetPreferencesAsync(string accessToken, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var url = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.PreferencesPath}";
        HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var httpResponse = await _httpClient.SendAsync(message, ct);
            if (httpResponse.IsSuccessStatusCode)
            {
                using Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
                FeedResponse? response = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.FeedResponse);
                return response?.Preferences is IReadOnlyList<PreferenceItem> { } preferences
                    ? Result.Ok(preferences)
                    : Result.Fail<IReadOnlyList<PreferenceItem>>("Preference array is null after deserialization");
            }
            else
            {
                var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                return Result.Fail<IReadOnlyList<PreferenceItem>>(errorMessage);
            }
        }
        catch (Exception e)
        {
            return Result.Fail<IReadOnlyList<PreferenceItem>>(e.Message);
        }
    }
}
