using System.Text.Json.Serialization;
using TCTOS.Impls.Incus.Conversion;
using TCTOS.Impls.Incus.Data;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record InstanceSource
{
    [JsonPropertyName("alias")] public string? Alias { get; set; }

    [JsonPropertyName("allow_inconsistent")]
    public bool? AllowInconsistent { get; set; }

    [JsonPropertyName("base-image")] public string? BaseImage { get; set; }

    [JsonPropertyName("certificate")] public string? Certificate { get; set; }

    [JsonPropertyName("fingerprint")] public string? Fingerprint { get; set; }

    [JsonPropertyName("instance_only")] public bool? InstanceOnly { get; set; }

    [JsonPropertyName("live")] public bool? Live { get; set; }

    [JsonPropertyName("mode")]
    [JsonConverter(typeof(StringToEnumConverter<TransferMode>))]
    public TransferMode? Mode { get; set; }

    [JsonPropertyName("operation")] public string? Operation { get; set; }

    [JsonPropertyName("project")] public string? Project { get; set; }

    [JsonPropertyName("properties")] public Dictionary<string, object>? Properties { get; set; }

    [JsonPropertyName("protocol")] public string? Protocol { get; set; }

    [JsonPropertyName("refresh")] public bool? Refresh { get; set; }

    [JsonPropertyName("refresh_exclude_order")]
    public bool? RefreshExcludeOrder { get; set; }

    [JsonPropertyName("secret")]
    public string? Secret { get; set; }
    
    [JsonPropertyName("secrets")]
    public Dictionary<string, object>? Secrets { get; set; }
    
    [JsonPropertyName("server")]
    public string? Server { get; set; }
    
    [JsonPropertyName("source")]
    public string? Source { get; set; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(StringToEnumConverter<ImageSourceType>))]
    public ImageSourceType? Type { get; set; }
}