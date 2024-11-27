using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class ProfileControlViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    private readonly IFeedItemViewModelFactory _feedItemViewModelFactory;

    public ProfileControlViewModel(
        IProfileService profileService,
        IFeedItemViewModelFactory feedItemViewModelFactory)
    {
        _profileService = profileService;
        _feedItemViewModelFactory = feedItemViewModelFactory;
    }

    public AuthorViewModel AuthorViewModel { get; } = new();

    public ObservableCollection<FeedItemViewModel> FeedItems { get; } = [];

    public async Task InitializeCurrentUserProfileAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Author? author = await _profileService.GetCurrentUserAsync();
        AuthorViewModel.SetAuthor(author);

        if (author?.Handle is not { Length: > 0 } handle)
        {
            return;
        }
        ct.ThrowIfCancellationRequested();

        var feedItems = await _profileService.GetProfileFeedAsync(handle);
        foreach (var f in feedItems)
        {
            ct.ThrowIfCancellationRequested();
            var vm = _feedItemViewModelFactory.CreateViewModel(f);
            FeedItems.Add(vm);
        }
    }
}
