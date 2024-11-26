using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Extensions;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    private MenuItem? _lastSelectedMenu;

    public ShellPageViewModel(
        ILocalizer localizer,
        ITelemetry telemetry,
        INavigator contentNavigator,
        INavigator rootNavigator,
        IProfileService profileService,
        IDialogService dialogService,
        IAuthenticationService authenticationService,
        IImageViewerService imageViewerService)
    {
        _localizer = localizer;
        _telemetry = telemetry;
        _contentNavigator = contentNavigator;
        _rootNavigator = rootNavigator;
        _profileService = profileService;
        _dialogService = dialogService;
        _authenticationService = authenticationService;
        _imageViewerService = imageViewerService;

        MenuItems.Add(new MenuItem(NavigateContentPageCommand, _localizer.GetString("HomeText"), "\uEA8A", NavigationConstants.HomePage));
        MenuItems.Add(new MenuItem(NavigateContentPageCommand, _localizer.GetString("NotificationsText"), "\uEA8F", NavigationConstants.NotificationsPage));
        MenuItems.Add(new MenuItem(NavigateContentPageCommand, _localizer.GetString("ProfileText"), "\uE77B", NavigationConstants.ProfilePage));
    }

    public ObservableCollection<MenuItem> MenuItems = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsImageViewerVisible))]
    private IReadOnlyList<ImageEmbed>? _images;

    public bool IsImageViewerVisible => Images is { Count: > 0 };

    [ObservableProperty]
    private int _imageViewerIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SafeAvatarUrl))]
    private Author _currentUser = new();

    public string SafeAvatarUrl => CurrentUser.SafeAvatarUrl();

    public async Task InitializeAsync(ShellPageNavigationArgs args)
    {
        bool shouldAbortToSignInPage;

        if (args.AlreadySignedIn)
        {
            string? testToken = await _authenticationService.TryGetFreshTokenAsync();
            shouldAbortToSignInPage = string.IsNullOrEmpty(testToken);
        }
        else
        {
            (bool signInSuccessful, string errorMessage) = await _authenticationService.TrySilentSignInAsync();
            shouldAbortToSignInPage = !signInSuccessful;

            if (signInSuccessful)
            {
                _telemetry.TrackEvent(TelemetryConstants.AuthSuccessFromShellPage);
            }
        }

        if (shouldAbortToSignInPage)
        {
            _telemetry.TrackEvent(TelemetryConstants.AuthFailFromShellPage);
            await _dialogService.OpenSignInRequiredAsync();
            _rootNavigator.NavigateTo(NavigationConstants.SignInPage);
            return;
        }

        _imageViewerService.ImageViewerRequested += OnImageViewerRequested;

        Task<Author?> profileTask = _profileService.GetCurrentUserAsync();
        NavigateContentPage(MenuItems[0]);
        CurrentUser = await profileTask ?? new();
    }

    public void Unitialize()
    {
        _imageViewerService.ImageViewerRequested -= OnImageViewerRequested;
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
