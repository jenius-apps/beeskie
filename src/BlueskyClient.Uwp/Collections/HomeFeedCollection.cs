using BlueskyClient.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace BlueskyClient.Collections;

public class HomeFeedCollection : ObservableCollection<FeedItemViewModel>, ISupportIncrementalLoading
{
    private readonly HomePageViewModel _viewModel;

    public HomeFeedCollection(HomePageViewModel homePageViewModel)
    {
        _viewModel = homePageViewModel;
        _viewModel.FeedItems.CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Add)
        {
            foreach (var newItem in e.NewItems)
            {
                if (newItem is FeedItemViewModel item)
                {
                    this.Add(item);
                }
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Remove)
        {
            this.RemoveAt(e.OldStartingIndex);
        }
    }

    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    {
        return AsyncInfo.Run(async cancelToken =>
        {
            var countAdded = await _viewModel.LoadNextPageAsync(cancelToken);
            return new LoadMoreItemsResult { Count = (uint)countAdded };
        });
    }

    public bool HasMoreItems => _viewModel.HasMoreItems;
}
