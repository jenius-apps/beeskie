using Bluesky.NET.Models;
using System.Runtime.CompilerServices;

namespace BlueskyClient.ViewModels;

public interface IFeedItemViewModelFactory
{
    FeedItemViewModel CreateViewModel(FeedItem feedItem, [CallerFilePath] string uiHostNameForTelemetry = "");

    FeedItemViewModel CreateViewModel(
        FeedPost post,
        FeedPostReason? reason = null,
        bool isPostThreadParent = false,
        [CallerFilePath] string uiHostNameForTelemetry = "");
}