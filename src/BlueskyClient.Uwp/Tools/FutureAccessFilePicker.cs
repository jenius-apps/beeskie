using BlueskyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

#nullable enable

namespace BlueskyClient.Tools.Uwp;

public sealed class FutureAccessFilePicker : IFutureAccessFilePicker
{
    /// <inheritdoc/>
    public async Task<FutureAccessImage?> PickFileAsync(IReadOnlyList<string> fileExtensionFilters)
    {
        FileOpenPicker picker = CreatePicker(fileExtensionFilters);
        StorageFile file = await picker.PickSingleFileAsync();

        if (file != null)
        {
            var token = StorageApplicationPermissions.FutureAccessList.Add(file);
            return new FutureAccessImage
            {
                Path = file.Path,
                FutureAccessToken = token
            };
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FutureAccessImage>> PickFilesAsync(IReadOnlyList<string> fileExtensionFilters)
    {
        FileOpenPicker picker = CreatePicker(fileExtensionFilters);
        var files = await picker.PickMultipleFilesAsync();

        if (files == null || files.Count == 0)
        {
            return [];
        }

        List<string> tokens = [];
        foreach (var file in files)
        {
            tokens.Add(StorageApplicationPermissions.FutureAccessList.Add(file));
        }

        return files.Select((x, index) => new FutureAccessImage { Path = x.Path, FutureAccessToken = tokens[index] }).ToArray();
    }

    private FileOpenPicker CreatePicker(IReadOnlyList<string> fileExtensionFilters)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };

        foreach (var filter in fileExtensionFilters)
        {
            if (filter.StartsWith('.'))
            {
                picker.FileTypeFilter.Add(filter);
            }
        }

        return picker;
    }
}
