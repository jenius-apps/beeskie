using System.Collections.Generic;
using System.Threading.Tasks;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Telemetry;
using JeniusApps.Common.Tools;

namespace BlueskyClient.ViewModels;

public partial class SignInPageViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;
    private readonly INavigator _navigator;
    private readonly IUserSettings _userSettings;
    private readonly ITelemetry _telemetry;

    public SignInPageViewModel(
        IAuthenticationService authenticationService,
        INavigator navigator,
        IUserSettings userSettings,
        ITelemetry telemetry)
    {
        _authService = authenticationService;
        _navigator = navigator;
        _userSettings = userSettings;
        _telemetry = telemetry;

        UserHandleInput = userSettings.Get<string>(UserSettingsConstants.LastUsedUserIdentifierInputKey) ?? string.Empty;
    }

    [ObservableProperty]
    private bool _signingIn;

    [ObservableProperty]
    private string _userHandleInput = string.Empty;

    [ObservableProperty]
    private string _appPasswordInput = string.Empty;

    [ObservableProperty]
    private string _pdsInput = "bsky.social";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorBannerVisible))]
    private string _signInErrorMessage = string.Empty;

    public bool ErrorBannerVisible => SignInErrorMessage.Length > 0;

    [RelayCommand]
    private async Task SignInAsync()
    {
        SigningIn = true;

        _telemetry.TrackEvent(TelemetryConstants.SignInClicked);

        Result<AuthResponse> result = await _authService.SignInAsync(UserHandleInput, AppPasswordInput, PdsInput.Contains("://") ? PdsInput : $"https://{PdsInput}");

        SignInErrorMessage = result.IsSuccess
            ? string.Empty
            : string.Join(", ", result.Errors);

        if (result.IsSuccess)
        {
            _userSettings.Set(UserSettingsConstants.LastUsedUserIdentifierInputKey, UserHandleInput);

            _navigator.NavigateTo(NavigationConstants.ShellPage, new ShellPageNavigationArgs
            {
                AlreadySignedIn = true
            });

            _telemetry.TrackEvent(TelemetryConstants.AuthSuccessFromSignInPage);
        }
        else
        {
            _telemetry.TrackEvent(TelemetryConstants.AuthFailFromSignInPage, new Dictionary<string, string>
            {
                { "errorMessage", SignInErrorMessage }
            });
        }

        SigningIn = false;
    }

    [RelayCommand]
    private void CloseError()
    {
        SignInErrorMessage = string.Empty;
    }
}
