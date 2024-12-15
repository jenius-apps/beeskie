namespace BlueskyClient.Models;

public sealed class SearchPageNavigationArgs
{
    public string? RequestedQuery { get; init; }

    public int RequestedSearchTabIndex { get; init; } // TODO switch to enum so it's safer
}
