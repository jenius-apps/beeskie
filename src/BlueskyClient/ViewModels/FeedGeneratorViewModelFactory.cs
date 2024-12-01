using System;
using Bluesky.NET.Models;

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
        return new FeedGeneratorViewModel(feedGenerator);
    }
}
