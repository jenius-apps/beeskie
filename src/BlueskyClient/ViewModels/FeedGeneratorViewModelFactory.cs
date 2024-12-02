using Bluesky.NET.Models;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlueskyClient.ViewModels;

public class FeedGeneratorViewModelFactory : IFeedGeneratorViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FeedGeneratorViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public FeedGeneratorViewModel Create(FeedGenerator feedGenerator)
    {
        return new FeedGeneratorViewModel(
            feedGenerator,
            _serviceProvider.GetRequiredService<ILocalizer>());
    }
}
