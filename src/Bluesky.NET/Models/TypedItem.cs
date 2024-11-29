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
