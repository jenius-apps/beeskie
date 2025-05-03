using System;

namespace BlueskyClient.Services;

/// <summary>
/// Interface for requesting a page refresh.
/// </summary>
public interface IRefreshPageRequester
{
    /// <summary>
    /// Raised when a refresh is requested.
    /// </summary>
    event EventHandler? RefreshRequested;

    /// <summary>
    /// Requests a refresh of the current content page.
    /// </summary>
    void RequestRefresh();
}