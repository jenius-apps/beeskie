using System.Collections.Generic;

namespace Bluesky.NET.Models;

/// <summary>
/// Interface for a resource with labels.
/// </summary>
public interface ILabeledResource
{
    /// <summary>
    /// List of labels on this resource.
    /// </summary>
    IReadOnlyList<Label>? Labels { get; init; }
}
