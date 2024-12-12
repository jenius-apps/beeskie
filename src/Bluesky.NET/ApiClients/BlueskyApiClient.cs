using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Bluesky.NET.Constants;
using Bluesky.NET.Models;
using FluentResults;

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

    private async Task<Result<T>> SendMessageAsync<T>(
        HttpRequestMessage message,
        JsonTypeInfo<T> jsonTypeInfo,
        CancellationToken ct)
    {
        try
        {
            var httpResponse = await _httpClient.SendAsync(message, ct);
            if (httpResponse.IsSuccessStatusCode)
            {
                using Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
                T? response = JsonSerializer.Deserialize(contentStream, jsonTypeInfo);
                return response is not null
                    ? Result.Ok(response)
                    : Result.Fail<T>("Deserialization result was null");
            }
            else
            {
                var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                return Result.Fail<T>(errorMessage);
            }
        }
        catch (Exception e)
        {
            return Result.Fail<T>(e.Message);
        }
    }

    private async Task<Result> SendMessageAsync(
        string accessToken,
        HttpRequestMessage message,
        CancellationToken ct)
    {
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var httpResponse = await _httpClient.SendAsync(message, ct);
            if (httpResponse.IsSuccessStatusCode)
            {
                return Result.Ok();
            }
            else
            {
                var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                return Result.Fail(errorMessage);
            }
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}
