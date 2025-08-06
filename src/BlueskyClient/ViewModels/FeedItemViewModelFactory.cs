using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Services;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace BlueskyClient.ViewModels;

public class FeedItemViewModelFactory : IFeedItemViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FeedItemViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public FeedItemViewModel CreateViewModel(FeedItem feedItem, [CallerFilePath] string uiHostNameForTelemetry = "")
    {
        return CreateViewModel(feedItem.Post, feedItem.Reason, uiHostNameForTelemetry: Path.GetFileName(uiHostNameForTelemetry));
    }

    public FeedItemViewModel CreateViewModel(
        FeedPost post,
        FeedPostReason? reason = null,
        bool isPostThreadParent = false,
        [CallerFilePath] string uiHostNameForTelemetry = "")
    {
        return new FeedItemViewModel(
            post,
            reason,
            _serviceProvider.GetRequiredService<IPostSubmissionService>(),
            _serviceProvider.GetRequiredService<IDialogService>(),
            _serviceProvider.GetRequiredService<ILocalizer>(),
            _serviceProvider.GetRequiredService<IAuthorViewModelFactory>(),
            _serviceProvider.GetRequiredKeyedService<INavigator>(NavigationConstants.ContentNavigatorKey),
            Path.GetFileName(uiHostNameForTelemetry),
            isPostThreadParent);
    }
}
