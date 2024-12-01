using Bluesky.NET.Models;

namespace BlueskyClient.ViewModels
{
    public interface IFeedGeneratorViewModelFactory
    {
        FeedGeneratorViewModel Create(FeedGenerator feedGenerator);
    }
}