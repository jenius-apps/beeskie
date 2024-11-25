using System.Threading.Tasks;

namespace BlueskyClient.Tools;

public interface IFileReadWriter
{
    Task<string?> CopyToLocalCacheAsync(string pathToFile);

    Task<byte[]?> GetByteArrayAsync(string pathToFile);

    Task<bool> DeleteLocalFileAsync(string pathToLocalFile);
}
