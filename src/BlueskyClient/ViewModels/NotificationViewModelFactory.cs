using Bluesky.NET.Models;
using JeniusApps.Common.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlueskyClient.ViewModels;

public class NotificationViewModelFactory : INotificationViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public NotificationViewModel CreateViewModel(Notification notification)
    {
        return new NotificationViewModel(
            notification,
            _serviceProvider.GetRequiredService<ILocalizer>());
    }
}
