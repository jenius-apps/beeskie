using System;

namespace BlueskyClient.Services;

/// <summary>
/// Class for requesting a refresh to be performed on the given content page.
/// </summary>
public sealed class RefreshPageRequester : IRefreshPageRequester
{
    /// <inheritdoc/>
    public event EventHandler? RefreshRequested;

    /// <inheritdoc/>
    public void RequestRefresh()
    {
        RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}