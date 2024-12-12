using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Constants;
using FluentResults;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Tools;
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
    public async Task<Result<AuthResponse>> TrySilentSignInAsync()
    {
#if DEBUG
        //return (false, "debugReturnFalse");
#endif
        string? storedDid = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey) ?? string.Empty;
        string? storedRefreshToken = _secureCredentialStorage.GetCredential(storedDid);
        if (storedRefreshToken is not { Length: > 0 })
        {
            return Result.Fail<AuthResponse>("Stored refresh token was empty.");
        }

        // Get a new token via the refresh token.
        Result<AuthResponse> result = await _apiClient.RefreshAsync(storedRefreshToken);
        if (result.IsSuccess)
        {
            UpdateStoredToken(result.Value);
        }

        // If the refresh token was already expired, then used the stored app password.
        string? storedAppPassword = _secureCredentialStorage.GetCredential(AppPasswordCredentialKey(storedDid));
        if (storedAppPassword is { Length: > 0 })
        {
            result = await SignInWithValidatedCredentialsAsync(storedDid, storedAppPassword);
        }

        return result;
    }

    /// <inheritdoc/>
    public void SignOut()
    {
        string? storedDid = _userSettings.Get<string>(UserSettingsConstants.SignedInDIDKey);
        if (storedDid is { Length: > 0 })
        {
            _secureCredentialStorage.SetCredential(storedDid, string.Empty);
            _secureCredentialStorage.SetCredential(AppPasswordCredentialKey(storedDid), string.Empty);
        }

        _userSettings.Set(UserSettingsConstants.LocalUserIdKey, string.Empty);
        _userSettings.Set(UserSettingsConstants.SignedInDIDKey, string.Empty);
        _accesToken = null;
        _refreshToken = null;
        _expirationTime = null;
    }

    /// <inheritdoc/>
    public async Task<Result<AuthResponse>> SignInAsync(string rawUserHandleOrEmail, string rawPassword)
    {
        var userHandleOrEmail = rawUserHandleOrEmail.Trim().TrimStart('@');
        var password = rawPassword.Trim();

        if (string.IsNullOrEmpty(userHandleOrEmail) || string.IsNullOrEmpty(password))
        {
            return Result.Fail<AuthResponse>("Empty identifier or password");
        }

        return await SignInWithValidatedCredentialsAsync(userHandleOrEmail, password);
    }

    public async Task<Result<string>> TryGetFreshTokenAsync()
    {
        if (_accesToken is null || _refreshToken is null || _expirationTime is null)
        {
            // Not initialized at all. Need to perform sign in.
            return Result.Fail<string>("Not initialized yet. Try authenticating explicitly or silently.");
        }

        // First, if the current token is expired, use the refresh token to retrieve a new token.
        // If the token isn't expired, then just skip this refresh.
        if (DateTime.Now >= _expirationTime.Value && _refreshToken is string refreshToken)
        {
            Result<AuthResponse> authResponse = await _apiClient.RefreshAsync(refreshToken);

            if (authResponse.IsSuccess)
            {
                UpdateStoredToken(authResponse.Value);
            }
        }

        // At this point, if the token is still valid, return it.
        if (DateTime.Now < _expirationTime.Value && _accesToken is string token)
        {
            return Result.Ok(token);
        }

        // If the refresh failed and the token is still not valid, do a full silient auth.
        var result = await TrySilentSignInAsync();
        if (result.IsSuccess && result.Value.AccessJwt is string newToken)
        {
            return Result.Ok(newToken);
        }

        return Result.Fail<string>(result.Errors);
    }

    private void UpdateStoredToken(AuthResponse response)
    {
        if (response is
            {
                Did: string { Length: > 0 } did,
                AccessJwt: string { Length: > 0 } accessToken,
                RefreshJwt: string { Length: > 0 } refreshToken
            })
        {
            _accesToken = accessToken;
            _refreshToken = refreshToken;
            _expirationTime = DateTime.Now.AddHours(TokenHoursToLive);
            _secureCredentialStorage.SetCredential(did, refreshToken);
        }
    }

    private async Task<Result<AuthResponse>> SignInWithValidatedCredentialsAsync(string identifier, string password)
    {
        Result<AuthResponse> result = await _apiClient.AuthenticateAsync(identifier, password);

        if (result.IsSuccess)
        {
            UpdateStoredToken(result.Value);

            if (result.Value is { Did: string { Length: > 0 } did })
            {
                _userSettings.Set(UserSettingsConstants.SignedInDIDKey, did);
                _secureCredentialStorage.SetCredential(AppPasswordCredentialKey(did), password);
            }
        }

        return result;
    }


    private static string AppPasswordCredentialKey(string did) => $"{did}-appPassword";
}
