using Bluesky.NET.Models;

namespace BlueskyClient.ViewModels;

public interface IFeedItemViewModelFactory
{
    FeedItemViewModel CreateViewModel(FeedItem feedItem);

    FeedItemViewModel CreateViewModel(
        FeedPost post,
        FeedPostReason? reason = null,
        bool canExpandThread = true);
}