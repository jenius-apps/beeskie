using Bluesky.NET.Models;
using BlueskyClient.Controls;
using JeniusApps.Common.Tools;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BlueskyClient.Services.Uwp;

public sealed class DialogService : IDialogService
{
    private readonly ILocalizer _localizer;
    private bool _dialogOpen;

    public DialogService(ILocalizer localizer)
    {
        _localizer = localizer;
    }

    public async Task OpenPostDialogAsync()
    {
        if (_dialogOpen)
        {
            return;
        }

        _dialogOpen = true;

        var dialog = new NewPostDialog();
        dialog.Initialize();
        await dialog.ShowAsync();

        _dialogOpen = false;
    }

    public async Task OpenReplyDialogAsync(FeedPost target)
    {
        if (_dialogOpen)
        {
            return;
        }

        _dialogOpen = true;

        var dialog = new NewPostDialog();
        dialog.Initialize(target);
        await dialog.ShowAsync();

        _dialogOpen = false;
    }

    /// <inheritdoc/>
    public async Task OpenSignInRequiredAsync()
    {
        if (_dialogOpen)
        {
            return;
        }

        _dialogOpen = true;

        var dialog = new ContentDialog
        {
            Title = _localizer.GetString("SessionTimeoutDialogTitle"),
            Content = _localizer.GetString("SessionTimeoutDialogMessage"),
            CloseButtonText = _localizer.GetString("OkayText")
        };

        await dialog.ShowAsync();

        _dialogOpen = false;
    }

    /// <inheritdoc/>
    public async Task<bool> LogoutAsync()
    {
        if (_dialogOpen)
        {
            return false;
        }

        _dialogOpen = true;

        var dialog = new ContentDialog
        {
            Title = _localizer.GetString("SignOutDialogTitle"),
            Content = _localizer.GetString("SignOutDialogMessage"),
            PrimaryButtonText = _localizer.GetString("SignOutText"),
            CloseButtonText = _localizer.GetString("CancelText"),
            DefaultButton = ContentDialogButton.Close
        };

        var result = await dialog.ShowAsync();

        _dialogOpen = false;

        return result is ContentDialogResult.Primary;
    }
}
