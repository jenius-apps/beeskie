using System;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class SubmissionRecord
{
    [JsonPropertyName("$type")]
    public string? Type { get; init; }

    public DateTime CreatedAt { get; init; }

    public string Text { get; init; } = string.Empty;

    public FeedPost? Subject { get; init; }

    public ReplyRecord? Reply { get; init; }

    public SubmissionEmbed? Embed { get; init; }

    public Facet[]? Facets { get; set; }
}

public class SubmissionEmbed
{
    [JsonPropertyName("$type")]
    public string? Type { get; init; }

    public SubmissionImageBlob[]? Images { get; init; }

    public FeedRecord? Record { get; init; }
}

public class SubmissionImageBlob
{
    public string Alt { get; init; } = string.Empty;

    public Blob? Image { get; init; }
}
