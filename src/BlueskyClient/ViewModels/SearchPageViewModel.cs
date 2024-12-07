using Bluesky.NET.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class SearchPageViewModel : ObservableObject
{
    private readonly ISearchService _searchService;
    private readonly IFeedItemViewModelFactory _feedItemFactory;
    private string? _cursor;

    public SearchPageViewModel(
        ISearchService searchService,
        IFeedItemViewModelFactory feedItemFactory)
    {
        _searchService = searchService;
        _feedItemFactory = feedItemFactory;
    }

    public ObservableCollection<FeedItemViewModel> SearchResults { get; } = [];

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private bool _searchLoading;

    public async Task InitializeAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await Task.Delay(1);
    }

    [RelayCommand]
    private async Task NewSearchAsync()
    {
        string query = Query.Trim();

        if (query is not { Length: > 0 })
        {
            return;
        }

        SearchLoading = true;

        _cursor = null;
        SearchResults.Clear();

        var (Posts, Cursor) = await _searchService.SearchPostsAsync(
           query,
           default,
           cursor: _cursor,
           options: null);

        _cursor = Cursor;

        foreach (var p in Posts)
        {
            var vm = _feedItemFactory.CreateViewModel(p, reason: null);
            SearchResults.Add(vm);
            SearchLoading = false;
        }
    }
}
