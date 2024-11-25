using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class Blob
{
    [JsonPropertyName("$type")]
    public string? Type { get; init; }

    public BlobRef? Ref { get; init; }

    public string? MimeType { get; init; }

    public double Size { get; init; }
}

public class BlobRef
{
    [JsonPropertyName("$link")]
    public string? Link { get; init; }
}
