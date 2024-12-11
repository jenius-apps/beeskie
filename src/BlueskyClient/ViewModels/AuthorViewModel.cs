using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Extensions;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class AuthorViewModel : ObservableObject
{
    private string _telemetryContext;
    private readonly IProfileService _profileService;
    private readonly ITelemetry _telemetry;
    private Author? _author;

    public AuthorViewModel(
        Author? author,
        IProfileService profileService,
        ITelemetry telemetry,
        string telemetryContext)
    {
        _author = author;
        _profileService = profileService;
        _telemetry = telemetry;
        _telemetryContext = telemetryContext;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AlreadyFollowed))]
    private bool _followSuccessful;

    public string DisplayName => _author?.DisplayName is { Length: > 0 } displayName
        ? displayName
        : _author?.Handle ?? string.Empty;

    public string AtHandle => _author?.AtHandle ?? string.Empty;

    public string Handle => _author?.Handle ?? string.Empty;

    public string Description => _author?.Description ?? string.Empty;

    public string AvatarUrl => _author.SafeAvatarUrl();

    public string BannerUrl => _author.SafeAvatarUrl();

    public Uri BannerUri => _author.SafeBannerUri();

    public string FollowersCount => _author.FollowersCount();

    public string FollowingCount => _author.FollowsCount();

    public string PostsCount => _author.PostsCount();

    public bool BannerVisible => !string.IsNullOrEmpty(_author?.Banner);

    public bool AlreadyFollowed => _author?.Viewer?.Following is { Length: > 0 } || FollowSuccessful;

    public void SetAuthor(Author? author)
    {
        _author = author;
        OnPropertyChanged(string.Empty);
    }

    [RelayCommand]
    private async Task FollowAsync(CancellationToken ct)
    {
        if (AlreadyFollowed || _author?.Did is not { Length: > 0 } subjectDid)
        {
            return;
        }

        FollowSuccessful = await _profileService.FollowActorAsync(subjectDid, ct);
        _telemetry.TrackEvent(TelemetryConstants.FollowCompleted, new Dictionary<string, string>
        {
            { "context", _telemetryContext }
        });
    }

    public override string ToString()
    {
        return $"{DisplayName}, {AtHandle}, {Description}";
    }
}
