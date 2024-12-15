using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using JeniusApps.Common.Models;
using JeniusApps.Common.Telemetry;
using JeniusApps.Common.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BlueskyClient.ViewModels;

public partial class ShellPageViewModel : ObservableObject
{
    private readonly ILocalizer _localizer;
    private readonly ITelemetry _telemetry;
    private readonly INavigator _contentNavigator;
    private readonly INavigator _rootNavigator;
    private readonly IProfileService _profileService;
    private readonly IDialogService _dialogService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IImageViewerService _imageViewerService;
    private readonly INotificationsService _notificationService;
    private MenuItem? _lastSelectedMenu;
    private readonly MenuItem _notificationMenuItem;

    public ShellPageViewModel(
        ILocalizer localizer,
        ITelemetry telemetry,
        INavigator contentNavigator,
        INavigator rootNavigator,
        IProfileService profileService,
        IDialogService dialogService,
        IAuthenticationService authenticationService,
        IImageViewerService imageViewerService,
        IAuthorViewModelFactory authorFactory,
        INotificationsService notificationsService)
    {
        AuthorViewModel = authorFactory.CreateStub();
        _localizer = localizer;
        _telemetry = telemetry;
        _contentNavigator = contentNavigator;
        _rootNavigator = rootNavigator;
        _profileService = profileService;
        _dialogService = dialogService;
        _authenticationService = authenticationService;
        _imageViewerService = imageViewerService;
        _notificationService = notificationsService;

        MenuItems.Add(new MenuItem(NavigateContentPageCommand, _localizer.GetString("HomeText"), "\uEA8A", NavigationConstants.HomePage));
        MenuItems.Add(new MenuItem(NavigateContentPageCommand, _localizer.GetString("SearchText"), "\uE721", NavigationConstants.SearchPage));
        _notificationMenuItem = new MenuItem(NavigateContentPageCommand, _localizer.GetString("NotificationsText"), "\uEA8F", NavigationConstants.NotificationsPage);
        MenuItems.Add(_notificationMenuItem);
        MenuItems.Add(new MenuItem(NavigateContentPageCommand, _localizer.GetString("ProfileText"), "\uE77B", NavigationConstants.ProfilePage));
#if DEBUG
        MenuItems.Add(new MenuItem(NavigateContentPageCommand, _localizer.GetString("FeedsText"), "\uF57F", NavigationConstants.FeedsPage));
#endif
    }

    private void OnContentPageNavigated(object sender, string key)
    {
        if (_lastSelectedMenu?.Tag == key)
        {
            return;
        }

        foreach (var item in MenuItems)
        {
            if (item.Tag == key)
            {
                item.IsSelected = true;
                _lastSelectedMenu = item;
            }
            else
            {
                item.IsSelected = false;
            }
        }
    }

    public AuthorViewModel AuthorViewModel { get; }

    public ObservableCollection<MenuItem> MenuItems { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsImageViewerVisible))]
    private IReadOnlyList<ImageEmbed>? _images;

    public bool IsImageViewerVisible => Images is { Count: > 0 };

    [ObservableProperty]
    private int _imageViewerIndex;

    public async Task InitializeAsync(ShellPageNavigationArgs args)
    {
        bool shouldAbortToSignInPage;
        string errorMessage = string.Empty;

        if (args.AlreadySignedIn)
        {
            Result<string> result = await _authenticationService.TryGetFreshTokenAsync();
            shouldAbortToSignInPage = result.IsFailed;
            errorMessage = string.Join(", ", result.Errors);
        }
        else
        {
            Result<AuthResponse> result = await _authenticationService.TrySilentSignInAsync();
            shouldAbortToSignInPage = result.IsFailed;
            errorMessage = string.Join(", ", result.Errors);

            if (result.IsSuccess)
            {
                _telemetry.TrackEvent(TelemetryConstants.AuthSuccessFromShellPage);
            }
        }

        if (shouldAbortToSignInPage)
        {
            _telemetry.TrackEvent(TelemetryConstants.AuthFailFromShellPage, new Dictionary<string, string>
            {
                { "errorMessage", errorMessage }
            });
            await _dialogService.OpenSignInRequiredAsync();
            _rootNavigator.NavigateTo(NavigationConstants.SignInPage);
            return;
        }

        _imageViewerService.ImageViewerRequested += OnImageViewerRequested;
        _contentNavigator.PageNavigated += OnContentPageNavigated;

        Task<Author?> profileTask = _profileService.GetCurrentUserAsync();
        NavigateContentPage(MenuItems[0]);
        AuthorViewModel.SetAuthor(await profileTask);

        _notificationMenuItem.BadgeCount = await _notificationService.GetUnreadCountAsync(default);
    }

    public void Unitialize()
    {
        _imageViewerService.ImageViewerRequested -= OnImageViewerRequested;
        _contentNavigator.PageNavigated -= OnContentPageNavigated;
    }

    private void OnImageViewerRequested(object sender, ImageViewerArgs args)
    {
        if (args.Images.Count == 0)
        {
            return;
        }

        _telemetry.TrackEvent(TelemetryConstants.ImageViewerOpened);

        ImageViewerIndex = args.LaunchIndex < args.Images.Count ? args.LaunchIndex : 0;
        Images = args.Images;
    }

    [RelayCommand]
    private void NavigateContentPage(MenuItem? item)
    {
        if (item?.Tag is not string { Length: > 0 } key)
        {
            return;
        }

        if (_lastSelectedMenu is { } lastMenu)
        {
            lastMenu.IsSelected = false;

            // If last menu was null, it means it's the first navigation to shell page.
            // For this telemetry, we don't care about the first navigation.
            // Hence, we only make the call when last menu isn't null, but those are subsequent navigations.
            _telemetry.TrackEvent(TelemetryConstants.MenuItemClicked, new Dictionary<string, string>
            {
                { "key", key }
            });
        }

        item.IsSelected = true;
        _lastSelectedMenu = item;

        if (key is NavigationConstants.NotificationsPage)
        {
            if (_notificationMenuItem.BadgeCount > 0)
            {
                _telemetry.TrackEvent(
                    TelemetryConstants.UserInteractedWithNotificationBadge,
                    new Dictionary<string, string>
                    {
                        { "badgeCount", _notificationMenuItem.BadgeCount.ToString() }
                    });
            }

            _notificationMenuItem.BadgeCount = 0;
        }

        _contentNavigator.NavigateTo(key);
    }

    [RelayCommand]
    private async Task NewPostAsync()
    {
        _telemetry.TrackEvent(TelemetryConstants.ShellNewPostClicked);
        await _dialogService.OpenPostDialogAsync();
    }

    [RelayCommand]
    private void CloseImageViewer(string? closeMethod)
    {
        Images = null;
        ImageViewerIndex = 0;

        _telemetry.TrackEvent(TelemetryConstants.ImageViewerClosed, new Dictionary<string, string>
        {
            { "closeMethod", closeMethod ?? "" }
        });
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var result = await _dialogService.LogoutAsync();
        if (result)
        {
            _authenticationService.SignOut();
            _rootNavigator.NavigateTo(NavigationConstants.SignInPage);
        }

        _telemetry.TrackEvent(TelemetryConstants.LogoutClicked, new Dictionary<string, string>
        {
            { "signedOut", result.ToString() }
        });
    }
}
