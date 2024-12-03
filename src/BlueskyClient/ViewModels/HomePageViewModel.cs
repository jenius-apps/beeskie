using BlueskyClient.Constants;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Telemetry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class HomePageViewModel : ObservableObject
{
    private readonly ITimelineService _timelineService;
    private readonly IFeedItemViewModelFactory _feedItemViewModelFactory;
    private readonly IFeedGeneratorService _feedGeneratorService;
    private readonly IFeedGeneratorViewModelFactory _feedGeneratorViewModelFactory;
    private readonly ITelemetry _telemetry;
    private string? _cursor;

    public HomePageViewModel(
        ITimelineService timelineService,
        IFeedItemViewModelFactory feedItemViewModelFactory,
        IFeedGeneratorService feedGeneratorService,
        IFeedGeneratorViewModelFactory feedGeneratorViewModelFactory,
        ITelemetry telemetry)
    {
        _timelineService = timelineService;
        _feedItemViewModelFactory = feedItemViewModelFactory;
        _feedGeneratorService = feedGeneratorService;
        _feedGeneratorViewModelFactory = feedGeneratorViewModelFactory;
        _telemetry = telemetry;
    }

    [ObservableProperty]
    private bool _feedLoading;

    [ObservableProperty]
    private FeedGeneratorViewModel? _selectedFeed;

    public bool HasMoreItems => _cursor is not null;

    public ObservableCollection<FeedGeneratorViewModel> Feeds { get; } = [];

    public ObservableCollection<FeedItemViewModel> FeedItems { get; } = [];

    public async Task InitializeAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        FeedLoading = true;

        var feedGenerators = await _feedGeneratorService.GetSavedFeedsAsync(true, ct);
        foreach (var f in feedGenerators)
        {
            var vm = _feedGeneratorViewModelFactory.Create(f);
            Feeds.Add(vm);
            SelectedFeed ??= vm;
        }

        await LoadNextPageAsync(ct);
    }

    public async Task<int> LoadNextPageAsync(CancellationToken ct)
    {
        if (_cursor is not null)
        {
            _telemetry.TrackEvent(TelemetryConstants.NextPageLoaded, new Dictionary<string, string>
            {
                { "currentFeed", SelectedFeed?.DisplayName ?? "none" }
            });
        }

        var (Items, Cursor) = SelectedFeed is { RawAtUri: string atUri, IsTimeline: false }
            ? await _timelineService.GetFeedItemsAsync(atUri, ct, _cursor)
            : await _timelineService.GetTimelineAsync(ct, _cursor);

        _cursor = Cursor;

        foreach (var item in Items)
        {
            var vm = _feedItemViewModelFactory.CreateViewModel(item);
            FeedItems.Add(vm);
            FeedLoading = false;
        }

        return Items.Count;
    }

    [RelayCommand]
    private async Task ChangeFeedsAsync(FeedGeneratorViewModel? vm)
    {
        if (vm is null || SelectedFeed == vm)
        {
            return;
        }

        _telemetry.TrackEvent(TelemetryConstants.FeedChanged, new Dictionary<string, string>
        {
            { "feedName", vm.DisplayName }
        });

        SelectedFeed = vm;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task RefreshFeedAsync()
    {
        _telemetry.TrackEvent(TelemetryConstants.HomeRefreshClicked);
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _cursor = null;
        FeedItems.Clear();
        FeedLoading = true;
        await LoadNextPageAsync(default);
    }
}