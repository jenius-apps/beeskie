using Bluesky.NET.Models;
using FluentResults;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IAuthenticationService
{
    Task<Result<AuthResponse>> SignInAsync(string rawUserHandle, string rawPassword, string? baseUrl);
    void SignOut();
    Task<Result<string>> TryGetFreshTokenAsync();
    Task<Result<AuthResponse>> TrySilentSignInAsync();
}