using Bluesky.NET.Models;
using System.Threading.Tasks;

namespace BlueskyClient.Services;

public interface IAuthenticationService
{
    Task<AuthResponse?> SignInAsync(string rawUserHandle, string rawPassword);
    void SignOut();
    Task<string?> TryGetFreshTokenAsync();
    Task<(bool, string)> TrySilentSignInAsync();
}