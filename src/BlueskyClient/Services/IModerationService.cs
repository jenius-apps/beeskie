using Bluesky.NET.Models;
using System.Collections.Generic;

namespace BlueskyClient.Services;

/// <summary>
/// Interface for moderating labeled resources.
/// </summary>
public interface IModerationService
{
    /// <summary>
    /// Returns true if the item is blocked.
    /// </summary>
    /// <typeparam name="T">A <see cref="ILabeledResource"/>.</typeparam>
    /// <param name="item">The resource to check.</param>
    /// <returns>True if the item is blocked, false otherwise.</returns>
    bool IsBlocked<T>(T item) where T : ILabeledResource;

    /// <summary>
    /// Filters the list and removes blocked items.
    /// </summary>
    /// <typeparam name="T">A <see cref="ILabeledResource"/>.</typeparam>
    /// <param name="unfilteredItems">List of items to filter.</param>
    /// <returns>Filtered list of items.</returns>
    IEnumerable<T> ModerateItems<T>(IReadOnlyList<T> unfilteredItems) where T : ILabeledResource;

    /// <summary>
    /// Filters the list and removes blocked items.
    /// </summary>
    /// <typeparam name="T">A <see cref="ILabeledResource"/>.</typeparam>
    /// <param name="unfilteredItems">List of items to filter.</param>
    /// <returns>Filtered list of items.</returns>
    IEnumerable<FeedItem> ModerateFeedItems(IReadOnlyList<FeedItem> unfilteredItems);
}