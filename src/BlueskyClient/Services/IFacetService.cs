using Bluesky.NET.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IFacetService
{
    /// <summary>
    /// Extracts facets from the given string.
    /// </summary>
    /// <param name="text">The plain text to extract facets from.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A list of facets.</returns>
    Task<IReadOnlyList<Facet>> ExtractFacetsAsync(string text, CancellationToken ct);
}