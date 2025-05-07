using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Linq;

namespace BlueskyClient.Services;

/// <summary>
/// Service to perform moderation on the list of items.
/// </summary>
public sealed class ModerationService : IModerationService
{
    private readonly HashSet<string> _blockList =
    [
        LabelValues.Hide,
        LabelValues.Porn,
        LabelValues.Sexual,
        LabelValues.GraphicMedia,
        LabelValues.Nudity
    ];

    /// <inheritdoc/>
    public IEnumerable<T> ModerateItems<T>(IReadOnlyList<T> unfilteredItems) where T : ILabeledResource
    {
        foreach (T item in unfilteredItems)
        {
            if (IsBlocked(item))
            {
                continue;
            }

            yield return item;
        }
    }

    /// <inheritdoc/>
    public bool IsBlocked<T>(T item) where T : ILabeledResource
    {
        return item.Labels.Any(x => _blockList.Contains(x.Val));
    }
}
