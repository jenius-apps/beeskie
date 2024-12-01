using Bluesky.NET.Models;
using BlueskyClient.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueskyClient.ViewModels;

public partial class FeedGeneratorViewModel : ObservableObject
{
    private readonly FeedGenerator _feedGenerator;

    public FeedGeneratorViewModel(FeedGenerator feedGenerator)
    {
        _feedGenerator = feedGenerator;
        AuthorViewModel.SetAuthor(feedGenerator.Creator);
    }

    public string DisplayName => _feedGenerator.IsTimeline
        ? "Following" // TODO consider making this change in the UI instead
        : _feedGenerator.DisplayName ?? string.Empty;

    public string FeedAvatar => _feedGenerator.SafeAvatarUrl();

    public AuthorViewModel AuthorViewModel { get; } = new();

    public string Description => _feedGenerator.Description ?? string.Empty;
}
