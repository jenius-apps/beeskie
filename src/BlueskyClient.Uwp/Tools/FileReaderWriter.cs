using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable enable

namespace BlueskyClient.Tools.Uwp;

public class FileReaderWriter : IFileReadWriter
{
    /// <inheritdoc/>
    public async Task<string?> CopyToLocalCacheAsync(string pathToFile)
    {
        if (string.IsNullOrEmpty(pathToFile))
        {
            return null;
        }

        StorageFile? copy;
        try
        {
            StorageFile original = await StorageFile.GetFileFromPathAsync(pathToFile);
            copy = await original.CopyAsync(ApplicationData.Current.LocalCacheFolder, original.Name, NameCollisionOption.ReplaceExisting);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            copy = null;
        }

        return copy?.Path;
    }

    /// <inheritdoc/>
    public async Task<byte[]?> GetByteArrayAsync(string pathToFile)
    {
        if (string.IsNullOrEmpty(pathToFile))
        {
            return null;
        }

        try
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(pathToFile);
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            return buffer.ToArray();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteLocalFileAsync(string pathToLocalFile)
    {
        if (string.IsNullOrEmpty(pathToLocalFile))
        {
            return false;
        }

        try
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(pathToLocalFile);
            await file.DeleteAsync();
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return false;
        }
    }

}
