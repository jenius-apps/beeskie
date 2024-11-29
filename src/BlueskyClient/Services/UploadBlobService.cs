using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Tools;
using FluentResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public sealed class UploadBlobService : IUploadBlobService
{
    private readonly IBlueskyApiClient _apiClient;
    private readonly IFileReadWriter _fileReadWriter;
    private readonly IAuthenticationService _authenticationService;

    public UploadBlobService(
        IBlueskyApiClient blueskyApiClient,
        IFileReadWriter fileReader,
        IAuthenticationService authenticationService)
    {
        _apiClient = blueskyApiClient;
        _fileReadWriter = fileReader;
        _authenticationService = authenticationService;
    }

    public async Task<IReadOnlyList<Blob?>> UploadBlobsAsync(
        IReadOnlyList<string> pathsToFiles,
        string mimeTypeForAll)
    {
        List<Blob?> result = [];
        foreach (var path in pathsToFiles)
        {
            var response = await UploadBlobAsync(path, mimeTypeForAll);
            result.Add(response);
        }

        return result;
    }

    public async Task<Blob?> UploadBlobAsync(string pathToFile, string mimeType)
    {
        if (pathToFile.Length < 0 || mimeType.Length < 0)
        {
            return null;
        }

        Result<string> tokenResult = await _authenticationService.TryGetFreshTokenAsync().ConfigureAwait(false);
        if (tokenResult.IsFailed)
        {
            return null;
        }

        // Caching required to avoid access denied issues related to uwp sandbox.
        string? cachedPath = await _fileReadWriter.CopyToLocalCacheAsync(pathToFile).ConfigureAwait(false);
        if (cachedPath is not { Length: > 0 })
        {
            return null;
        }

        byte[]? bytes = await _fileReadWriter.GetByteArrayAsync(cachedPath).ConfigureAwait(false);
        if (bytes is null)
        {
            return null;
        }

        var result = await _apiClient.UploadBlobAsync(tokenResult.Value, bytes, mimeType).ConfigureAwait(false);
        await _fileReadWriter.DeleteLocalFileAsync(cachedPath).ConfigureAwait(false);

        return result;
    }
}
