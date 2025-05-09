namespace BlueskyClient.Models;

/// <summary>
/// Arguments used for initializing profile controls.
/// </summary>
public sealed class ProfileNavigationArgs
{
    /// <summary>
    /// The requested author object's DID identifier to initialize in the profile control.
    /// </summary>
    public string? AuthorDid { get; init; }
}
