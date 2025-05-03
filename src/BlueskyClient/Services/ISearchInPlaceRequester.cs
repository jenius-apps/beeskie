using BlueskyClient.Models;
using System;

namespace BlueskyClient.Services;

/// <summary>
/// Interface for requesting a search to be performed
/// if the search page is already present.
/// </summary>
public interface ISearchInPlaceRequester
{
    /// <summary>
    /// Raised when a search request is made.
    /// </summary>
    event EventHandler<SearchPageNavigationArgs>? SearchRequested;

    /// <summary>
    /// Requests a search to be performed.
    /// </summary>
    /// <param name="query">The search to perform.</param>
    void RequestSearch(string query);
}