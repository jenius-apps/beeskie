using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IDiscoverService
{
    /// <summary>
    /// Retrieves list of suggested people to follow.
    /// </summary>
    /// <param name="ct">A cancellation token. </param>
    /// <param name="count">Number of people to retrieve.</param>
    /// <param name="cursor">A string used to fetch the next page.</param>
    /// <returns>List of suggested people to follow, and a cursor for pagination.</returns>
    Task<(IReadOnlyList<Author> Authors, string? cursor)> GetSuggestedPeopleAsync(
        CancellationToken ct,
        int count = 10,
        string? cursor = null);
}