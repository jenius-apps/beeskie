using Bluesky.NET.Models;
using BlueskyClient.ViewModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface INotificationsService
{
    Task<IReadOnlyList<Notification>> GetNotificationsAsync();

    /// <summary>
    /// Retrieves the unread notifications count for the user.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>Int representing the unread count.</returns>
    Task<int> GetUnreadCountAsync(CancellationToken ct);
    Task HydrateAsync(NotificationViewModel notification);
}