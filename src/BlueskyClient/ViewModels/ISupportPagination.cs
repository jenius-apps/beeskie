using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

/// <summary>
/// Interface designed to be used with viewmodels
/// to expose common methods and properties to be used by
/// UWP's incremental loading features.
/// </summary>
/// <typeparam name="T">The object type that populates the paginated list.</typeparam>
public interface ISupportPagination<T>
{
    /// <summary>
    /// The source of truth for items.
    /// </summary>
    ObservableCollection<T> CollectionSource { get; }

    /// <summary>
    /// Indicates if more items are available to be fetched.
    /// </summary>
    bool HasMoreItems { get; }

    /// <summary>
    /// Runs the logic to fetch the next page.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The number of new items added.</returns>
    Task<int> LoadNextPageAsync(CancellationToken ct);
}
