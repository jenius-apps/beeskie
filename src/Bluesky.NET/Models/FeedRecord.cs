using System;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class FeedRecord
{
    [JsonPropertyName("$type")]
    public string? Type { get; init; }

    public string? Uri { get; init; }

    public string? Cid { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAtUtc { get; init; }

    public string? Description { get; init; }

    public string? Name { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public string? Text { get; init; }

    public ReplyRecord? Reply { get; init; }

    public PostEmbed[]? Embeds { get; init; }

    public Author? Author { get; init; }

    public FeedRecord? Value { get; init; } // fyi for future, I think this could be anything, not necessarily a record

    public FeedRecord? Record { get; init; }

    public Author? Creator { get; init; }

    public Facet[]? Facets { get; init; }
}
