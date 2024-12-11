using Bluesky.NET.Models;
using BlueskyClient.Services;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlueskyClient.ViewModels;

public class FeedItemViewModelFactory : IFeedItemViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FeedItemViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public FeedItemViewModel CreateViewModel(FeedItem feedItem)
    {
        return CreateViewModel(feedItem.Post, feedItem.Reason);
    }

    public FeedItemViewModel CreateViewModel(FeedPost post, FeedPostReason? reason = null)
    {
        return new FeedItemViewModel(
            post,
            reason,
            _serviceProvider.GetRequiredService<IPostSubmissionService>(),
            _serviceProvider.GetRequiredService<IDialogService>(),
            _serviceProvider.GetRequiredService<ILocalizer>(),
            _serviceProvider.GetRequiredService<IAuthorViewModelFactory>());
    }
}
