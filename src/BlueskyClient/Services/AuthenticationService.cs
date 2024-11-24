using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using BlueskyClient.Tools;
using JeniusApps.Common.Settings;
using System;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private const int TokenHoursToLive = 2; // based on decoded JWT, TTL is 2 hours.
    private readonly IBlueskyApiClient _apiClient;
    private readonly ISecureCredentialStorage _secureCredentialStorage;
    private readonly IUserSettings _userSettings;
    private string? _accesToken;
    private string? _refreshToken;
    private DateTime? _expirationTime;

    public AuthenticationService(
        IBlueskyApiClient blueskyApiClient,
        ISecureCredentialStorage secureCredentialStorage,
        IUserSettings userSettings)
    {
        _apiClient = blueskyApiClient;
        _secureCredentialStorage = secureCredentialStorage;
        _userSettings = userSettings;
    }

    /// <inheritdoc/>
    public async Task<(bool, string)> TrySilentSignInAsync()
    {
#if DEBUG
        //return (false, "debugReturnFalse");
#endif
        string? storedUserHandle = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey) ?? string.Empty;
        string? storedRefreshToken = _secureCredentialStorage.GetCredential(storedUserHandle);
        if (storedRefreshToken is not { Length: > 0 })
        {
            return (false, "emptyStoredRefreshToken");
        }

        var authResponse = await _apiClient.RefreshAsync(storedRefreshToken);
        UpdateStoredToken(authResponse);

        return (authResponse?.Success is true, authResponse?.ErrorMessage ?? string.Empty);
    }

    /// <inheritdoc/>
    public void SignOut()
    {
        string? storedDid = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey);
        if (storedDid is { Length: > 0 })
        {
            _secureCredentialStorage.SetCredential(storedDid, string.Empty);
        }

        _userSettings.Set(UserSettingsConstants.LocalUserIdKey, string.Empty);
        _userSettings.Set(UserSettingsConstants.SignedInDIDKey, string.Empty);
        _accesToken = null;
        _refreshToken = null;
        _expirationTime = null;
    }

    /// <inheritdoc/>
    public async Task<AuthResponse?> SignInAsync(string rawUserHandleOrEmail, string rawPassword)
    {
        var userHandleOrEmail = rawUserHandleOrEmail.Trim().TrimStart('@');
        var password = rawPassword.Trim();

        if (string.IsNullOrEmpty(userHandleOrEmail) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var result = await _apiClient.AuthenticateAsync(userHandleOrEmail, password);
        UpdateStoredToken(result);

        if (result is { Success: true, Did: string { Length: > 0 } did })
        {
            _userSettings.Set(UserSettingsConstants.SignedInDIDKey, did);
        }

        return result;
    }

    public async Task<string?> TryGetFreshTokenAsync()
    {
        if (_accesToken is null || _refreshToken is null || _expirationTime is null)
        {
            // Not initialized at all. Need to perform sign in.
            return null;
        }

        if (DateTime.Now >= _expirationTime.Value && _refreshToken is string refreshToken)
        {
            var authResponse = await _apiClient.RefreshAsync(refreshToken);
            UpdateStoredToken(authResponse);
        }

        if (DateTime.Now < _expirationTime.Value && _accesToken is string token)
        {
            return token;
        }

        // Everything failed, so return null.
        return null;
    }

    private void UpdateStoredToken(AuthResponse? response)
    {
        if (response is { 
                Success: true,
                Did: string { Length: > 0 } did,
                AccessJwt: string { Length: > 0 } accessToken,
                RefreshJwt: string { Length: > 0 } refreshToken })
        {
            _accesToken = accessToken;
            _refreshToken = refreshToken;
            _expirationTime = DateTime.Now.AddHours(TokenHoursToLive);
            _secureCredentialStorage.SetCredential(did, refreshToken);
        }
    }
}
