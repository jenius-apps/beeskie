using Bluesky.NET.Models;

namespace BlueskyClient.Models;

/// <summary>
/// Arguments used for initializing profile controls.
/// </summary>
public sealed class ProfileNavigationArgs
{
    /// <summary>
    /// The requested author object to initialize in the profile control.
    /// </summary>
    public Author? Author { get; init; }
}
