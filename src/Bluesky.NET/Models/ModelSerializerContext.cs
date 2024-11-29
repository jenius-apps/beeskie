using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

[JsonSerializable(typeof(AuthResponse))]
[JsonSerializable(typeof(AuthRequestBody))]
[JsonSerializable(typeof(PreferenceItem[]), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(FeedMetaData[]), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(FeedItem[]), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(FeedPost[]), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(Notification[]), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(FeedResponse), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(TypedItem), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(PreferenceItemSavedFeeds), GenerationMode = JsonSourceGenerationMode.Metadata)] // Only used to deserialize
[JsonSerializable(typeof(FeedRecord[]))]
[JsonSerializable(typeof(Author))]
[JsonSerializable(typeof(Notification))]
[JsonSerializable(typeof(CreateRecordBody))]
[JsonSerializable(typeof(CreateRecordResponse))]
[JsonSerializable(typeof(PostEmbed))]
[JsonSerializable(typeof(Viewer))]
[JsonSerializable(typeof(SubmissionRecord))]
[JsonSerializable(typeof(ReplyRecord))]
[JsonSerializable(typeof(FeedPostReason))]
[JsonSerializable(typeof(ImageEmbed))]
[JsonSerializable(typeof(AspectRatio))]
[JsonSerializable(typeof(BlobRef))]
[JsonSerializable(typeof(Blob))]
[JsonSerializable(typeof(SubmissionImageBlob))]
public sealed partial class ModelSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// The lazily initialized backing field for the context to be used for case insensitive serialization (<see cref="CaseInsensitive"/>).
    /// </summary>
    private static ModelSerializerContext? _caseInsensitive;

    /// <summary>
    /// A case insensitive variant of <see cref="Default"/>.
    /// </summary>
    public static ModelSerializerContext CaseInsensitive => _caseInsensitive ??= new ModelSerializerContext(new JsonSerializerOptions(s_defaultOptions) 
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    });
}
