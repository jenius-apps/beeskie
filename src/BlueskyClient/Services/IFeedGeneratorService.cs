using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluesky.NET.Models;

namespace BlueskyClient.Services;

public interface IFeedGeneratorService
{
    /// <summary>
    /// Retrieves the list of saved feeds.
    /// </summary>
    /// <param name="pinnedFeedsOnly">
    /// If true, the result will be filtered to pinned feeds.
    /// if false, all feeds will be returned.
    /// </param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>List of feeds.</returns>
    Task<IReadOnlyList<FeedGenerator>> GetSavedFeedsAsync(bool pinnedFeedsOnly, CancellationToken ct);
}