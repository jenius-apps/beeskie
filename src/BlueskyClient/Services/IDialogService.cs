using Bluesky.NET.Models;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IDialogService
{
    Task<bool> LogoutAsync();
    Task OpenPostDialogAsync();

    Task OpenReplyDialogAsync(FeedPost target);

    Task OpenSignInRequiredAsync();
}
