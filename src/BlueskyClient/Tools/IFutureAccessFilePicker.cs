using BlueskyClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueskyClient.Tools;

public interface IFutureAccessFilePicker
{
    Task<FutureAccessImage?> PickFileAsync(IReadOnlyList<string> fileExtensionFilters);

    Task<IReadOnlyList<FutureAccessImage>> PickFilesAsync(IReadOnlyList<string> fileExtensionFilters);
}
