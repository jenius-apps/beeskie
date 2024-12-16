using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class Facet
{
    public IndexData? Index { get; init; }

    public FacetFeature[]? Features { get; init; }
}

/// <summary>
/// An index range that has an inclusive start and an exclusive end. That means the end number goes 1 past what you might expect.
/// Exclusive-end helps the math stay consistent. If you subtract the end from the start, you get the correct length of the target string.
/// In this case, 15-6 = 9, which is the length of the "this site" string.
/// </summary>
public class IndexData
{
    /// <summary>
    /// Inclusive start in the byte array (not in the string position).
    /// </summary>
    public int ByteStart { get; init; }

    /// <summary>
    /// Exclusive end in the byte array (not in the string position).
    /// </summary>
    public int ByteEnd { get; init; }

    [JsonIgnore]
    public int Length => ByteEnd - ByteStart;
}

public class FacetFeature : TypedItem
{
    public string? Tag { get; init; }

    public string? Did { get; init; }

    public string? Uri { get; init; }

    [JsonIgnore]
    public FacetFeatureType FeatureType => Type switch
    {
        string type when type.EndsWith("tag") => FacetFeatureType.Tag,
        string type when type.EndsWith("link") => FacetFeatureType.Link,
        string type when type.EndsWith("mention") => FacetFeatureType.Mention,
        _ => FacetFeatureType.Unknown
    };
}

public enum FacetFeatureType
{
    /// <summary>
    /// Unknown facet type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Facet is a URL link. The facet feature Uri property should be populated.
    /// </summary>
    Link,

    /// <summary>
    /// Facet is a mention. The facet feature DID property should be populated.
    /// </summary>
    Mention,

    /// <summary>
    /// Facet is a hashtag. The facet feature Tag proprety should be populated.
    /// </summary>
    Tag
}
