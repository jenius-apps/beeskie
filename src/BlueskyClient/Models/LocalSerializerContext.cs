using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueskyClient.Models;

[JsonSerializable(typeof(string[]))]
public sealed partial class LocalSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// The lazily initialized backing field for the context to be used for case insensitive serialization (<see cref="CaseInsensitive"/>).
    /// </summary>
    private static LocalSerializerContext? _caseInsensitive;

    /// <summary>
    /// A case insensitive variant of <see cref="Default"/>.
    /// </summary>
    public static LocalSerializerContext CaseInsensitive => _caseInsensitive ??= new LocalSerializerContext(new JsonSerializerOptions(s_defaultOptions)
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    });
}
