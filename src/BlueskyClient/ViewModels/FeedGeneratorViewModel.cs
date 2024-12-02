using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using JeniusApps.Common.Tools;

namespace BlueskyClient.ViewModels;

public partial class FeedGeneratorViewModel : ObservableObject
{
    private readonly FeedGenerator _feedGenerator;
    private readonly ILocalizer _localizer;

    public FeedGeneratorViewModel(
        FeedGenerator feedGenerator,
        ILocalizer localizer)
    {
        _feedGenerator = feedGenerator;
        _localizer = localizer;
        AuthorViewModel.SetAuthor(feedGenerator.Creator);
    }

    public string? RawAtUri => _feedGenerator.Uri;

    public bool IsTimeline => _feedGenerator.IsTimeline;

    public string DisplayName => _feedGenerator.IsTimeline
        ? _localizer.GetString("FollowingFeedName")
        : _feedGenerator.DisplayName ?? string.Empty;

    public string FeedAvatar => _feedGenerator.SafeAvatarUrl();

    public AuthorViewModel AuthorViewModel { get; } = new();

    public string Description => _feedGenerator.Description ?? string.Empty;

    public override string ToString()
    {
        return DisplayName;
    }
}
