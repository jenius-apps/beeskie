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
    private readonly string _telemetryContext;
    private readonly IProfileService _profileService;
    private readonly ITelemetry _telemetry;

    public AuthorViewModel(
        Author? author,
        IProfileService profileService,
        ITelemetry telemetry,
        string telemetryContext)
    {
        Author = author;
        _profileService = profileService;
        _telemetry = telemetry;
        _telemetryContext = telemetryContext;
    }

    /// <summary>
    /// The <see cref="Bluesky.NET.Models.Author"/> associated with this viewmodel.
    /// </summary>
    public Author? Author { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AlreadyFollowed))]
    private bool _followSuccessful;

    public string DisplayName => Author?.DisplayName is { Length: > 0 } displayName
        ? displayName
        : Author?.Handle ?? string.Empty;

    public string AtHandle => Author?.AtHandle ?? string.Empty;

    public string Handle => Author?.Handle ?? string.Empty;

    public string Description => Author?.Description ?? string.Empty;

    public string AvatarUrl => Author.SafeAvatarUrl();

    public string BannerUrl => Author.SafeAvatarUrl();

    public Uri BannerUri => Author.SafeBannerUri();

    public string FollowersCount => Author.FollowersCount();

    public string FollowingCount => Author.FollowsCount();

    public string PostsCount => Author.PostsCount();

    public bool BannerVisible => !string.IsNullOrEmpty(Author?.Banner);

    public bool AlreadyFollowed => Author?.Viewer?.Following is { Length: > 0 } || FollowSuccessful;

    public void SetAuthor(Author? author)
    {
        Author = author;
        OnPropertyChanged(string.Empty);
    }

    [RelayCommand]
    private async Task FollowAsync(CancellationToken ct)
    {
        if (AlreadyFollowed || Author?.Did is not { Length: > 0 } subjectDid)
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
