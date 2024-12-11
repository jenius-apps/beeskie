using System;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class Author
{
    public string? Did { get; init; }

    public string? Handle { get; init; }

    public string? DisplayName { get; init; }

    public string? Avatar { get; init; }

    public string? Banner { get; init; }

    public string? Description { get; init; }

    public int? FollowersCount { get; init; }

    public int? FollowsCount { get; init; }

    public int? PostsCount { get; init; }

    public ViewerData? Viewer { get; init; }

    [JsonIgnore]
    public string AtHandle => Handle is null ? string.Empty : $"@{Handle}";
}
