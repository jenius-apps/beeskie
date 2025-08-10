using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bluesky.NET.ApiClients;

partial class BlueskyApiClient
{
    public async Task<Blob?> UploadBlobAsync(string accessToken, byte[] blob, string mimeType)
    {
        var url = $"{_baseUrl}/{UrlConstants.UploadBlobPath}";

        HttpRequestMessage message = new(HttpMethod.Post, url)
        {
            Content = new ByteArrayContent(blob)
        };
        message.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await _httpClient.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                var responseRecord = JsonSerializer.Deserialize(contentStream, ModelSerializerContext.CaseInsensitive.FeedResponse);
                return responseRecord?.Blob;
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
}
