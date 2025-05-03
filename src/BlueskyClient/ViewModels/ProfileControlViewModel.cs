using Bluesky.NET.Models;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class ProfileControlViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    private readonly IFeedItemViewModelFactory _feedItemViewModelFactory;

    public ProfileControlViewModel(
        IProfileService profileService,
        IFeedItemViewModelFactory feedItemViewModelFactory,
        IAuthorViewModelFactory authorFactory)
    {
        _profileService = profileService;
        _feedItemViewModelFactory = feedItemViewModelFactory;
        AuthorViewModel = authorFactory.CreateStub();
    }

    public AuthorViewModel AuthorViewModel { get; }

    public ObservableCollection<FeedItemViewModel> FeedItems { get; } = [];

    public async Task InitializeAsync(ProfileNavigationArgs? args, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Author? author = args?.Author ?? await _profileService.GetCurrentUserAsync();
        AuthorViewModel.SetAuthor(author);

        if (author?.Handle is not { Length: > 0 } handle)
        {
            return;
        }
        cancellationToken.ThrowIfCancellationRequested();

        FeedItems.Clear();
        var feedItems = await _profileService.GetProfileFeedAsync(handle);
        foreach (var f in feedItems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var vm = _feedItemViewModelFactory.CreateViewModel(f);
            FeedItems.Add(vm);
        }
    }
}
