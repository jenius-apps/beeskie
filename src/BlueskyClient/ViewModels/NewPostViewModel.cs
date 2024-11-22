using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class NewPostViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    private readonly IPostSubmissionService _postSubmissionService;
    private readonly ITelemetry _telemetry;

    public NewPostViewModel(
        IProfileService profileService,
        IPostSubmissionService postSubmissionService,
        ITelemetry telemetry)
    {
        _profileService = profileService;
        _postSubmissionService = postSubmissionService;
        _telemetry = telemetry;
    }

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Avatar))]
    [NotifyPropertyChangedFor(nameof(Handle))]
    private Author? _author;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TargetName))]
    [NotifyPropertyChangedFor(nameof(TargetText))]
    [NotifyPropertyChangedFor(nameof(TargetAvatar))]
    private FeedPost? _targetPost;

    public string Avatar => Author?.Avatar is string { Length: > 0 } avatarUri && Uri.IsWellFormedUriString(avatarUri, UriKind.Absolute)
        ? avatarUri
        : "http://localhost";

    public string Handle => Author?.AtHandle ?? string.Empty;

    public string TargetName => TargetPost?.Author?.DisplayName ?? string.Empty;

    public string TargetText => TargetPost?.Record?.Text ?? string.Empty;

    public string TargetAvatar => TargetPost?.Author?.Avatar is string { Length: > 0 } avatarUri && Uri.IsWellFormedUriString(avatarUri, UriKind.Absolute)
        ? avatarUri
        : "http://localhost";

    public async Task InitializeAsync(FeedPost? targetPost = null)
    {
        TargetPost = targetPost;
        Author = await _profileService.GetCurrentUserAsync();
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        _telemetry.TrackEvent(TelemetryConstants.PostSubmissionClicked);

        var input = InputText.Trim();
        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        string? newPostAtUri;
        if (TargetPost is { } target)
        {
            newPostAtUri = await _postSubmissionService.ReplyAsync(input, target).ConfigureAwait(false);
        }
        else
        {
            newPostAtUri = await _postSubmissionService.SubmitPostAsync(input).ConfigureAwait(false);
        }

        _telemetry.TrackEvent(TargetPost is null ? TelemetryConstants.NewPostSubmitted : TelemetryConstants.ReplySubmitted, new Dictionary<string, string>
        {
            { "success", (!string.IsNullOrEmpty(newPostAtUri)).ToString() }
        });
    }
}
