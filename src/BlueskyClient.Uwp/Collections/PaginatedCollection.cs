using BlueskyClient.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace BlueskyClient.Collections;

public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
{
    private readonly ISupportPagination<T> _paginationSource;

    public PaginatedCollection(ISupportPagination<T> source)
    {
        _paginationSource = source;
        source.CollectionSource.CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Add)
        {
            foreach (var newItem in e.NewItems)
            {
                if (newItem is T item)
                {
                    this.Add(item);
                }
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Remove)
        {
            this.RemoveAt(e.OldStartingIndex);
        }
        else if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            this.Clear();
        }
    }

    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    {
        return AsyncInfo.Run(async cancelToken =>
        {
            var countAdded = await _paginationSource.LoadNextPageAsync(cancelToken);
            return new LoadMoreItemsResult { Count = (uint)countAdded };
        });
    }

    public bool HasMoreItems => _paginationSource.HasMoreItems;
}
