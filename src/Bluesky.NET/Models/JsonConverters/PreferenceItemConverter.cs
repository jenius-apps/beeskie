using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models.JsonConverters;

// Ref: https://makolyte.com/csharp-deserialize-json-to-a-derived-type/

public class PreferenceItemConverter : JsonConverter<PreferenceItem>
{
    public override PreferenceItem? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using JsonDocument jsonDoc = JsonDocument.ParseValue(ref reader);
        var typeString = jsonDoc.RootElement.GetProperty("$type").GetString();
        return typeString switch
        {
            PreferenceItem.SavedFeedsKey => jsonDoc.RootElement.Deserialize<PreferenceItemSavedFeeds>(options),
            _ => new PreferenceItem { Type = typeString },
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        PreferenceItem value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}
