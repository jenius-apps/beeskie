using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public abstract class TypedItem
{
    [JsonPropertyName("$type")]
    public string? Type { get; init; }
}

public static class FacetTypes
{
    private const string FacetBaseType = "app.bsky.richtext.facet";
    public const string Link = FacetBaseType + "#link";
    public const string Mention = FacetBaseType + "#mention";
    public const string Tag = FacetBaseType + "#tag";
}
