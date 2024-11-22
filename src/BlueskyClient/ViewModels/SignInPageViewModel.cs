using BlueskyClient.Constants;
using BlueskyClient.Models;
using BlueskyClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Telemetry;
using JeniusApps.Common.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    [NotifyPropertyChangedFor(nameof(ErrorBannerVisible))]
    private string _signInErrorMessage = string.Empty;

    public bool ErrorBannerVisible => SignInErrorMessage.Length > 0;

    [RelayCommand]
    private async Task SignInAsync()
    {
        SigningIn = true;

        _telemetry.TrackEvent(TelemetryConstants.SignInClicked);

        var result = await _authService.SignInAsync(UserHandleInput, AppPasswordInput);

        SignInErrorMessage = result?.Success is true
            ? string.Empty
            : result?.ErrorMessage ?? "Null response";

        if (result?.Success is true)
        {
            _userSettings.Set(UserSettingsConstants.LastUsedUserIdentifierInputKey, UserHandleInput);

            _navigator.NavigateTo(NavigationConstants.ShellPage, new ShellPageNavigationArgs 
            { 
                AlreadySignedIn = true 
            });
        }

        SigningIn = false;

        _telemetry.TrackEvent(
            result?.Success is true ? TelemetryConstants.AuthSuccessFromSignInPage : TelemetryConstants.AuthFailFromSignInPage,
            new Dictionary<string, string>
            {
                { "userInputContainsAtSymbol", UserHandleInput.Contains("@").ToString() },
                { "handleContainsAtSymbol", result?.Handle?.Contains("@").ToString() ?? "NullHandle" },
            });
    }
}
