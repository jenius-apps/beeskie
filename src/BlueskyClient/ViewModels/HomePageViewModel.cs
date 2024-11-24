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
    private string? _cursor;

    public HomePageViewModel(
        ITimelineService timelineService,
        IFeedItemViewModelFactory feedItemViewModelFactory)
    {
        _timelineService = timelineService;
        _feedItemViewModelFactory = feedItemViewModelFactory;
    }

    public bool HasMoreItems => _cursor is not null;

    public ObservableCollection<FeedItemViewModel> FeedItems { get; } = [];

    public async Task InitializeAsync(CancellationToken ct)
    {
        await LoadNextPageAsync(ct);
    }

    public async Task<int> LoadNextPageAsync(CancellationToken ct)
    {
        var (Items, Cursor) = await _timelineService.GetTimelineAsync(ct, _cursor);
        _cursor = Cursor;

        foreach (var item in Items)
        {
            var vm = _feedItemViewModelFactory.CreateViewModel(item);
            FeedItems.Add(vm);
        }

        return Items.Count;
    }
}