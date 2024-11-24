using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface ITimelineService
{
    Task<(IReadOnlyList<FeedItem> Items, string? Cursor)> GetTimelineAsync(CancellationToken ct, string? cursor = null);
}