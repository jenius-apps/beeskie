using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueskyClient.Services
{
    public interface IUploadBlobService
    {
        Task<Blob?> UploadBlobAsync(string pathToFile, string mimeType);
        Task<IReadOnlyList<Blob?>> UploadBlobsAsync(IReadOnlyList<string> pathsToFiles, string mimeTypeForAll);
    }
}