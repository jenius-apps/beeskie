using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class ExternalEmbed
{
    public string Uri { get; init; } = string.Empty;

    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? Thumb { get; init; }

    [JsonIgnore]
    public string RootUri => System.Uri.TryCreate(Uri, UriKind.Absolute, out var uri)
        ? $"{uri.Scheme}://{uri.Host}"
        : string.Empty;
}
