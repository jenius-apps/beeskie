using System;
using System.Collections.Generic;

namespace BlueskyClient.Constants;

public sealed class UserSettingsConstants
{
    /// <summary>
    /// Remembers the signed-in DID identifier.
    /// </summary>
    public const string SignedInDIDKey = "SignedInDID";

    /// <summary>
    /// Remembers the handle or email used by the user to sign in.
    /// </summary>
    public const string LastUsedUserIdentifierInputKey = "LastUsedUserIdentifierInputKey";

    /// <summary>
    /// Anonymous ID referencing the local user, for telemetry purposes.
    /// </summary>
    public const string LocalUserIdKey = "LocalUserId";

    /// <summary>
    /// Stores recent searches by the user.
    /// </summary>
    public const string RecentSearchesKey = "RecentSearches";

    /// <summary>
    ///  Settings defaults.
    /// </summary>
    public static IReadOnlyDictionary<string, object> Defaults { get; } = new Dictionary<string, object>()
    {
        { LastUsedUserIdentifierInputKey, string.Empty },
        { SignedInDIDKey, string.Empty },
        { RecentSearchesKey, Array.Empty<string>() },
    };
}
