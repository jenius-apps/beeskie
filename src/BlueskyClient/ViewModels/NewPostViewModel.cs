using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Models;
using BlueskyClient.Services;
using BlueskyClient.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class NewPostViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    private readonly IPostSubmissionService _postSubmissionService;
    private readonly ITelemetry _telemetry;
    private readonly IFutureAccessFilePicker _picker;

    public NewPostViewModel(
        IProfileService profileService,
        IPostSubmissionService postSubmissionService,
        ITelemetry telemetry,
        IFutureAccessFilePicker picker)
    {
        _profileService = profileService;
        _postSubmissionService = postSubmissionService;
        _telemetry = telemetry;
        _picker = picker;
    }

    public bool ImageListVisible => Images.Count > 0;

    public ObservableCollection<FutureAccessImage> Images { get; } = [];

    public bool IsLowCharactersRemaining => CharactersRemaining is > 0 and <= 50;

    public bool IsNoCharactersRemaining => CharactersRemaining is <= 0;

    public int CharactersRemaining => 300 - InputText.Length;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CharactersRemaining))]
    [NotifyPropertyChangedFor(nameof(IsNoCharactersRemaining))]
    [NotifyPropertyChangedFor(nameof(IsLowCharactersRemaining))]
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

    public bool CanSubmit()
    {
        return _postSubmissionService.ValidatePost(InputText);
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (!CanSubmit())
        {
            return;
        }

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
        else if (Images.Count > 0)
        {
            newPostAtUri = await _postSubmissionService.SubmitPostWithImagesAsync(
                input,
                Images.Select(x => x.Path).ToArray())
                .ConfigureAwait(false);
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

    [RelayCommand]
    private async Task AddImageAsync()
    {
        if (Images.Count >= 4)
        {
            return;
        }
        
        FutureAccessImage? futureAccessImage = await _picker.PickFileAsync([".jpg", ".jpeg", ".png", ".webp", ".gif"]);
        if (futureAccessImage is { } image)
        {
            Images.Add(image);
        }

        OnPropertyChanged(nameof(ImageListVisible));
    }
}
