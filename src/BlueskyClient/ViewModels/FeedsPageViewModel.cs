using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueskyClient.ViewModels;

public partial class FeedsPageViewModel : ObservableObject
{
    private readonly IFeedGeneratorService _feedGeneratorService;
    private readonly IFeedGeneratorViewModelFactory _feedGeneratorViewModelFactory;

    public FeedsPageViewModel(
        IFeedGeneratorService feedGeneratorService,
        IFeedGeneratorViewModelFactory feedGeneratorViewModelFactory)
    {
        _feedGeneratorService = feedGeneratorService;
        _feedGeneratorViewModelFactory = feedGeneratorViewModelFactory;
    }

    public ObservableCollection<FeedGeneratorViewModel> Feeds { get; } = [];

    public async Task InitializeAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var result = await _feedGeneratorService.GetSavedFeedsAsync(pinnedFeedsOnly: false, ct);
        foreach (var item in result)
        {
            var vm = _feedGeneratorViewModelFactory.Create(item);
            Feeds.Add(vm);
        }
    }
}
