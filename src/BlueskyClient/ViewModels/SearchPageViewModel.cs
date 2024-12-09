using Bluesky.NET.ApiClients;
using Bluesky.NET.Constants;
using BlueskyClient.Constants;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Telemetry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class SearchPageViewModel : ObservableObject, ISupportPagination<FeedItemViewModel>
{
    private readonly ISearchService _searchService;
    private readonly IFeedItemViewModelFactory _feedItemFactory;
    private readonly ITelemetry _telemetry;
    private string? _cursor;
    private string? _currentQuery;
    private SearchOptions? _currentOptions;

    public SearchPageViewModel(
        ISearchService searchService,
        IFeedItemViewModelFactory feedItemFactory,
        ITelemetry telemetry)
    {
        _searchService = searchService;
        _feedItemFactory = feedItemFactory;
        _telemetry = telemetry;

        RecentSearches.CollectionChanged += OnRecentSearchesCollectionChanged;
    }

    public bool RecentSearchPlaceholderVisible => RecentSearches.Count == 0;

    [ObservableProperty]
    private int _searchTabIndex = 0;

    [ObservableProperty]
    private bool _searchPagePlaceholderVisible = true;

    /// <inheritdoc/>
    public ObservableCollection<FeedItemViewModel> CollectionSource { get; } = [];

    /// <summary>
    /// List of user's recent searches.
    /// </summary>
    public ObservableCollection<RecentSearchViewModel> RecentSearches { get; } = [];

    /// <inheritdoc/>
    public bool HasMoreItems => _cursor is not null;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private bool _searchLoading;

    public async Task InitializeAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _searchService.RecentSearchAdded += OnRecentSearchAdded;

        await Task.Delay(1);

        var recentSearches = _searchService.GetRecentSearches();
        foreach (var r in recentSearches)
        {
            RecentSearches.Add(new RecentSearchViewModel(r, RunRecentSearchCommand, DeleteRecentSearchCommand));
        }
    }

    public void Uninitialize()
    {
        _searchService.RecentSearchAdded -= OnRecentSearchAdded;
    }

    [RelayCommand]
    private Task RunRecentSearchAsync(string? query, CancellationToken ct)
    {
        if (query is not { Length: > 0 } || query == Query)
        {
            return Task.CompletedTask;
        }

        _telemetry.TrackEvent(TelemetryConstants.RecentSearchClicked);

        Query = query;
        return NewSearchAsync(ct);
    }

    [RelayCommand]
    private void DeleteRecentSearch(RecentSearchViewModel? vm)
    {
        if (vm?.Query is { Length: > 0 } query)
        {
            _searchService.DeleteRecentSearch(query);
            RecentSearches.Remove(vm);

            _telemetry.TrackEvent(TelemetryConstants.RecentSearchDeleted);
        }
    }

    public async Task<int> LoadNextPageAsync(CancellationToken ct)
    {
        if (_currentQuery is null)
        {
            return 0;
        }

        if (_cursor is not null)
        {
            _telemetry.TrackEvent(TelemetryConstants.SearchNextPageLoaded);
        }

        _currentOptions ??= new()
        {
            Sort = SearchTabIndex switch
            {
                0 => SearchConstants.SortTop,
                1 => SearchConstants.SortLatest,
                _ => SearchConstants.SortTop
            }
        };

        var (Posts, Cursor) = await _searchService.SearchPostsAsync(
           _currentQuery,
           ct,
           cursor: _cursor,
           options: _currentOptions);

        _cursor = Cursor;

        foreach (var p in Posts)
        {
            var vm = _feedItemFactory.CreateViewModel(p, reason: null);
            CollectionSource.Add(vm);
            SearchLoading = false;
        }

        return Posts.Count;
    }


    [RelayCommand]
    private async Task NewSearchAsync(CancellationToken ct)
    {
        string query = Query.Trim();

        if (query is not { Length: > 0 })
        {
            return;
        }

        SearchPagePlaceholderVisible = false;
        SearchLoading = true;
        _cursor = null;
        CollectionSource.Clear();
        _currentQuery = query;
        _currentOptions = null;

        await LoadNextPageAsync(ct);
        _telemetry.TrackEvent(TelemetryConstants.SearchTriggered);
    }

    async partial void OnSearchTabIndexChanged(int value)
    {
        NewSearchCommand.Cancel();
        await NewSearchCommand.ExecuteAsync(null);
        _telemetry.TrackEvent(TelemetryConstants.SearchTabClicked, new Dictionary<string, string>
        {
            { "newIndex", value.ToString() }
        });
    }

    private void OnRecentSearchesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(RecentSearchPlaceholderVisible));
    }

    private void OnRecentSearchAdded(object sender, string newQuery)
    {
        RecentSearches.Insert(0, new RecentSearchViewModel(newQuery, RunRecentSearchCommand, DeleteRecentSearchCommand));
    }
}
