using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bluesky.NET.ApiClients;

public partial class BlueskyApiClient : IBlueskyApiClient
{
    private readonly HttpClient _httpClient = new();

    public async Task<Result<AuthResponse>> RefreshAsync(string refreshToken)
    {
        var refreshUrl = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.RefreshAuthPath}";
        HttpRequestMessage message = new(HttpMethod.Post, refreshUrl);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
        return await PostAuthMessageAsync(message);
    }

    /// <inheritdoc/>
    public async Task<Result<AuthResponse>> AuthenticateAsync(string identifer, string appPassword)
    {
        var authUrl = $"{UrlConstants.BlueskyBaseUrl}/{UrlConstants.AuthPath}";

        var requestBody = new AuthRequestBody
        {
            Identifier = identifer,
            Password = appPassword
        };

        HttpRequestMessage message = new(HttpMethod.Post, authUrl)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(requestBody, ModelSerializerContext.CaseInsensitive.AuthRequestBody),
                Encoding.UTF8,
                "application/json")
        };

        return await PostAuthMessageAsync(message);
    }

    private async Task<Result<AuthResponse>> PostAuthMessageAsync(HttpRequestMessage message)
    {
        try
        {
            var response = await _httpClient.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Fail<AuthResponse>(errorContent);
            }

            using Stream resultStream = await response.Content.ReadAsStreamAsync();
            var authResponse = await JsonSerializer.DeserializeAsync(
                resultStream,
                ModelSerializerContext.CaseInsensitive.AuthResponse);

            return authResponse is null
                ? Result.Fail<AuthResponse>("Null deserialization")
                : Result.Ok(authResponse);
        }
        catch (Exception e)
        {
            return Result.Fail<AuthResponse>(e.Message);
        }
    }
}
