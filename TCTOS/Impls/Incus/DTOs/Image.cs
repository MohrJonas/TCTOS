using System.Text.Json.Serialization;
using TCTOS.Impls.Incus.Conversion;
using TCTOS.Impls.Incus.Data;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record Image
{
    [JsonPropertyName("aliases")] public ImageAlias[]? Aliases { init; get; }

    [JsonPropertyName("architecture")] public string? Architecture { init; get; }

    [JsonPropertyName("auto_update")] public bool? AutoUpdate { init; get; }

    [JsonPropertyName("cached")] public bool? Cached { init; get; }

    [JsonPropertyName("created_at")] public DateTime? CreatedAt { init; get; }

    [JsonPropertyName("expires_at")] public DateTime? ExpiresAt { init; get; }

    [JsonPropertyName("filename")] public string? FileName { init; get; }

    [JsonPropertyName("fingerprint")] public string? Fingerprint { init; get; }

    [JsonPropertyName("last_used_at")] public DateTime? LastUsedAt { init; get; }

    [JsonPropertyName("profiles")] public string[]? Profiles { init; get; }

    [JsonPropertyName("project")] public string? Project { init; get; }

    [JsonPropertyName("properties")] public Dictionary<string, string>? Properties { init; get; }

    [JsonPropertyName("public")] public bool? Public { init; get; }

    [JsonPropertyName("size")] public long? Size { init; get; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(StringToEnumConverter<ImageType>))]
    public ImageType? ImageType { init; get; }

    [JsonPropertyName("update_source")] public ImageSource? ImageSource { init; get; }

    [JsonPropertyName("uploaded_at")] public DateTime? UploadedAt { init; get; }
}