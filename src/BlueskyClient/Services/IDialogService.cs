using Bluesky.NET.Models;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IDialogService
{
    Task<bool> LogoutAsync();
    Task OpenPostDialogAsync();

    Task OpenReplyDialogAsync(FeedPost target);

    /// <summary>
    /// Opens the new post dialog with quote mode enabled.
    /// </summary>
    /// <param name="target">The post to quote.</param>
    Task OpenQuoteDialogAsync(FeedPost target);

    Task OpenSignInRequiredAsync();
}
