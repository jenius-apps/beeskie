using System;

namespace BlueskyClient.Helpers;

public static class UriStringHelper
{
    public static ReadOnlySpan<char> GetRecordKey(string targetUri)
    {
        var uriSpan = targetUri.AsSpan();
        var lastSlashIndex = uriSpan.LastIndexOf('/');

        return lastSlashIndex != -1 ? uriSpan[(lastSlashIndex + 1)..] : (ReadOnlySpan<char>)null;
    }
}
