﻿using Bluesky.NET.ApiClients;
using BlueskyClient.Constants;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Telemetry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using SearchConstants = Bluesky.NET.Constants.SearchConstants;

namespace BlueskyClient.ViewModels;

public partial class SearchPageViewModel : ObservableObject, ISupportPagination<FeedItemViewModel>, ISupportPagination<AuthorViewModel>, ISupportPagination<FeedGeneratorViewModel>
{
    private readonly ISearchService _searchService;
    private readonly IFeedItemViewModelFactory _feedItemFactory;
    private readonly ITelemetry _telemetry;
    private readonly IDiscoverService _discoverService;
    private readonly IAuthorViewModelFactory _authorViewModelFactory;
    private readonly IFeedGeneratorViewModelFactory _feedGeneratorViewModelFactory;
    private readonly ISearchInPlaceRequester _searchInPlaceRequester;
    private string? _cursor;
    private string? _currentQuery;
    private SearchOptions? _currentOptions;
    private bool _initialized;

    public SearchPageViewModel(
        ISearchService searchService,
        IFeedItemViewModelFactory feedItemFactory,
        ITelemetry telemetry,
        IDiscoverService discoverService,
        IAuthorViewModelFactory authorViewModelFactory,
        IFeedGeneratorViewModelFactory feedGeneratorViewModelFactory,
        ISearchInPlaceRequester searchInPlaceRequester)
    {
        _searchService = searchService;
        _feedItemFactory = feedItemFactory;
        _telemetry = telemetry;
        _discoverService = discoverService;
        _authorViewModelFactory = authorViewModelFactory;
        _feedGeneratorViewModelFactory = feedGeneratorViewModelFactory;
        _searchInPlaceRequester = searchInPlaceRequester;

        RecentSearches.CollectionChanged += OnRecentSearchesCollectionChanged;
    }

    public bool RecentSearchPlaceholderVisible => RecentSearches.Count == 0;

    public bool FeedsResultsVisible => !SearchLoading && SearchTabIndex == 3;

    public bool ActorsResultsVisible => !SearchLoading && SearchTabIndex == 2;

    public bool PostsResultsVisible => !SearchLoading && SearchTabIndex < 2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FeedsResultsVisible))]
    [NotifyPropertyChangedFor(nameof(ActorsResultsVisible))]
    [NotifyPropertyChangedFor(nameof(PostsResultsVisible))]
    private int _searchTabIndex = 0;

    [ObservableProperty]
    private bool _searchPagePlaceholderVisible = true;

    /// <inheritdoc/>
    public ObservableCollection<FeedItemViewModel> CollectionSource { get; } = [];

    public ObservableCollection<RecentSearchViewModel> RecentSearches { get; } = [];

    public ObservableCollection<AuthorViewModel> SuggestedPeople { get; } = [];

    public ObservableCollection<AuthorViewModel> ActorsCollectionSource { get; } = [];

    public ObservableCollection<FeedGeneratorViewModel> FeedsCollectionSrouce { get; } = [];

    /// <inheritdoc/>
    public bool HasMoreItems => _cursor is not null;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FeedsResultsVisible))]
    [NotifyPropertyChangedFor(nameof(ActorsResultsVisible))]
    [NotifyPropertyChangedFor(nameof(PostsResultsVisible))]
    private bool _searchLoading;

    public async Task InitializeAsync(SearchPageNavigationArgs? args, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!_initialized)
        {
            _initialized = true;
            _searchService.RecentSearchAdded += OnRecentSearchAdded;
            _searchInPlaceRequester.SearchRequested += OnSearchRequested;
        }

        var recentSearches = _searchService.GetRecentSearches();
        foreach (var r in recentSearches)
        {
            RecentSearches.Add(new RecentSearchViewModel(r, RunRecentSearchCommand, DeleteRecentSearchCommand));
        }

        if (args is null)
        {
            var (Authors, _) = await _discoverService.GetSuggestedPeopleAsync(ct, count: 5);
            foreach (var a in Authors)
            {
                SuggestedPeople.Add(_authorViewModelFactory.Create(a, telemetryContext: "suggestedPeople"));
            }
        }
        else
        {
            Query = args.RequestedQuery ?? string.Empty;
            SearchTabIndex = args.RequestedSearchTabIndex;
            await NewSearchCommand.ExecuteAsync(null);
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

    private async void OnSearchRequested(object sender, SearchPageNavigationArgs e)
    {
        if (e.RequestedQuery is not { Length: > 0 } query || query == Query)
        {
            return;
        }

        Query = query;
        await NewSearchAsync(default);
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

        return SearchTabIndex switch
        {
            3 => await LoadNextFeedsAsync(_currentQuery, ct),
            2 => await LoadNextActorsAsync(_currentQuery, ct),
            _ => await LoadNextPostsAsync(_currentQuery, ct)
        };
    }

    private async Task<int> LoadNextFeedsAsync(string validatedQuery, CancellationToken ct)
    {
        var (Feeds, Cursor) = await _searchService.SearchFeedsAsync(
           validatedQuery,
           ct,
           cursor: _cursor);

        _cursor = Cursor;
        SearchLoading = false;

        foreach (var feed in Feeds)
        {
            var vm = _feedGeneratorViewModelFactory.Create(feed);
            FeedsCollectionSrouce.Add(vm);
        }

        return Feeds.Count;
    }

    private async Task<int> LoadNextActorsAsync(string validatedQuery, CancellationToken ct)
    {
        var (Actors, Cursor) = await _searchService.SearchActorsAsync(
           validatedQuery,
           ct,
           cursor: _cursor);

        _cursor = Cursor;
        SearchLoading = false;

        foreach (var p in Actors)
        {
            var vm = _authorViewModelFactory.Create(p, telemetryContext: "searchResults");
            ActorsCollectionSource.Add(vm);
        }

        return Actors.Count;
    }

    private async Task<int> LoadNextPostsAsync(string validatedQuery, CancellationToken ct)
    {
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
           validatedQuery,
           ct,
           cursor: _cursor,
           options: _currentOptions);

        _cursor = Cursor;
        SearchLoading = false;

        foreach (var p in Posts)
        {
            var vm = _feedItemFactory.CreateViewModel(p, reason: null);
            CollectionSource.Add(vm);
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
        ActorsCollectionSource.Clear();
        FeedsCollectionSrouce.Clear();
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

        int lastIndex = RecentSearches.Count - 1;
        while (lastIndex > Constants.SearchConstants.RecentSearchMaxCount - 1)
        {
            RecentSearches.RemoveAt(lastIndex);
            lastIndex = RecentSearches.Count - 1;
        }
    }
}
