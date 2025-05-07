using System;

namespace Bluesky.NET.Models;

// Ref: https://atproto.com/specs/label

/// <summary>
/// A form of metadata about any account or content in the atproto ecosystem.
/// </summary>
public class Label
{
    /// <summary>
    /// DID of the actor who created this label.
    /// </summary>
    public required string Src { get; init; }

    /// <summary>
    /// AT URI of the record, user, or other resource that this label applies to.
    /// </summary>
    public required string Uri { get; init; }

    /// <summary>
    /// CID specifying the specific version of URI resource this label applies to.
    /// </summary>
    public string? Cid { get; init; }

    /// <summary>
    /// The short string name of the value or type of this label. See <see cref="LabelValues"/>
    /// for known options.
    /// </summary>
    /// <remarks>
    /// Ref: https://docs.bsky.app/docs/advanced-guides/moderation#global-label-values
    /// </remarks>
    public required string Val { get; init; }

    /// <summary>
    /// If true, indicates that this label "negates" an earlier label with the same src, uri, and val.
    /// </summary>
    public bool? Neg { get; init; }

    /// <summary>
    /// Timestamp when this label was created.
    /// </summary>
    public required DateTime Cts { get; init; }

    /// <summary>
    /// The label's schema version.
    /// </summary>
    public int Ver { get; init; }
}

/// <summary>
/// Protocol-level known label values.
/// </summary>
public sealed class LabelValues
{
    /// <summary>
    /// A generic warning on content that cannot be clicked through,
    /// and filters the content from listings. Not configurable by the user.
    /// </summary>
    public const string Hide = "!hide";

    /// <summary>
    /// A generic warning on content but can be clicked through. Not configurable by the user.
    /// </summary>
    public const string Warn = "!warn";

    /// <summary>
    /// Makes the content inaccessible to logged-out users in applications which respect the label.
    /// </summary>
    public const string NoUnauthenticated = "!no-unauthenticated";

    /// <summary>
    /// A warning on images and can only be clicked through if the user is 18+ and has enabled adult content.
    /// </summary>
    public const string Porn = "porn";

    /// <summary>
    /// Behaves like porn but is meant to handle less intense sexual content.
    /// </summary>
    public const string Sexual = "sexual";

    /// <summary>
    /// Behaves like porn but is for violence/gore.
    /// </summary>
    public const string GraphicMedia = "graphic-media";

    /// <summary>
    /// A warning on images but isn’t 18+ and defaults to ignore.
    /// </summary>
    public const string Nudity = "nudity";
}