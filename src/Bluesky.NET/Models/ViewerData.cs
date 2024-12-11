namespace Bluesky.NET.Models;

public class ViewerData
{
    public bool? Muted { get; init; }

    public bool? BlockedBy { get; init; }

    public string? Blocking { get; init; }

    public string? Following { get; init; }

    public string? FollowedBy { get; init; }
}
