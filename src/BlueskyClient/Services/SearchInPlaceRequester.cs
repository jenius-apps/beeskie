using BlueskyClient.Models;
using System;

namespace BlueskyClient.Services;

/// <summary>
/// Class for requesting a search to be performed in-place.
/// </summary>
/// <remarks>
/// This implicitly assumes that the search page is already active
/// so that it can perform the requested search. This does not
/// perform logic to navigate to the search page.
/// </remarks>
public sealed class SearchInPlaceRequester : ISearchInPlaceRequester
{
    /// <inheritdoc/>
    public event EventHandler<SearchPageNavigationArgs>? SearchRequested;

    /// <inheritdoc/>
    public void RequestSearch(string query)
    {
        if (query is not { Length: > 0 })
        {
            return;
        }

        SearchRequested?.Invoke(this, new SearchPageNavigationArgs
        {
            RequestedQuery = query
        });
    }
}
