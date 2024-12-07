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
        return new FeedItemViewModel(
            feedItem.Post,
            feedItem.Reason,
            _serviceProvider.GetRequiredService<IPostSubmissionService>(),
            _serviceProvider.GetRequiredService<IDialogService>(),
            _serviceProvider.GetRequiredService<ILocalizer>());
    }
}
