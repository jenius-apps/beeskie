using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class Facet
{
    public IndexData? Index { get; init; }

    public FacetFeature[]? Features { get; init; }
}

/// <summary>
/// An index range that has an inclusive start and an exclusive end. That means the end number goes 1 past what you might expect.
/// Exclusive-end helps the math stay consistent. If you subtract the end from the start, you get the correct length of the target string. In this case, 15-6 = 9, which is the length of the "this site" string.
/// </summary>
public class IndexData
{
    /// <summary>
    /// Inclusive start.
    /// </summary>
    public int ByteStart { get; init; }

    /// <summary>
    /// Exclusive end.
    /// </summary>
    public int ByteEnd { get; init; }
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
    Unknown,

    Link,

    Mention,

    Tag
}
