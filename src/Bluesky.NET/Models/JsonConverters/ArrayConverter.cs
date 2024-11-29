using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models.JsonConverters;

// Ref: https://stackoverflow.com/a/76282333/10953422

public class ArrayConverter<T, TConverter> : JsonConverter<T[]>
    where TConverter : JsonConverter<T>, new()
{
    private readonly JsonConverter<T> itemConverter = new TConverter();

    public override T[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            reader.Skip();
            return null;
        }

        var list = new List<T>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var item = this.itemConverter.Read(ref reader, typeof(T), options);
            if (item != null)
            {
                list.Add(item);
            }
        }

        return [.. list];
    }

    public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (T item in value)
        {
            this.itemConverter.Write(writer, item, options);
        }

        writer.WriteEndArray();
    }
}