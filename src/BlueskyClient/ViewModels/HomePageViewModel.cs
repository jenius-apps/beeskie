using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
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
    private string? _cursor;

    public HomePageViewModel(
        ITimelineService timelineService,
        IFeedItemViewModelFactory feedItemViewModelFactory,
        IFeedGeneratorService feedGeneratorService,
        IFeedGeneratorViewModelFactory feedGeneratorViewModelFactory)
    {
        _timelineService = timelineService;
        _feedItemViewModelFactory = feedItemViewModelFactory;
        _feedGeneratorService = feedGeneratorService;
        _feedGeneratorViewModelFactory = feedGeneratorViewModelFactory;
    }

    [ObservableProperty]
    private FeedGeneratorViewModel? _selectedFeed;

    public bool HasMoreItems => _cursor is not null;

    public ObservableCollection<FeedGeneratorViewModel> FeedGenerators { get; } = [];

    public ObservableCollection<FeedItemViewModel> FeedItems { get; } = [];

    public async Task InitializeAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var feedGenerators = await _feedGeneratorService.GetSavedFeedsAsync(true, ct);
        foreach (var f in feedGenerators)
        {
            var vm = _feedGeneratorViewModelFactory.Create(f);
            FeedGenerators.Add(vm);
            SelectedFeed ??= vm;
        }

        await LoadNextPageAsync(ct);
    }

    public async Task<int> LoadNextPageAsync(CancellationToken ct)
    {
        var (Items, Cursor) = SelectedFeed is { RawAtUri: string atUri, IsTimeline: false }
            ? await _timelineService.GetFeedItemsAsync(atUri, ct, _cursor)
            : await _timelineService.GetTimelineAsync(ct, _cursor);

        _cursor = Cursor;

        foreach (var item in Items)
        {
            var vm = _feedItemViewModelFactory.CreateViewModel(item);
            FeedItems.Add(vm);
        }

        return Items.Count;
    }
}