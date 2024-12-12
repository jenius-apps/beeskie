namespace Bluesky.NET.Models;

public class UpdateSeenBody
{
    /// <summary>
    /// The datetime when the notifications were seen.
    /// </summary>
    /// <remarks>
    /// Use this formatting: DateTime.Now.ToUniversalTime().ToString("O").
    /// Example result: 2024-12-12T16:25:57.513Z
    /// </remarks>
    public required string SeenAt { get; init; }
}
