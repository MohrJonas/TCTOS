using System.Text.Json.Serialization;
using TCTOS.Impls.Incus.Conversion;
using TCTOS.Impls.Incus.Data;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record ImagesPost
{
    [JsonPropertyName("aliases")]
    public ImageAlias[]? Aliases { get; set; }
    
    [JsonPropertyName("auto_update")]
    public bool? AutoUpdate { get; set; }
    
    [JsonPropertyName("compression_algorithm")]
    public string? CompressionAlgorithm { get; set; }
    
    [JsonPropertyName("expires_at")]
    public DateTime? ExpiresAt { get; set; }
    
    [JsonPropertyName("filename")]
    public string? FileName { get; set; }
    
    [JsonPropertyName("format")]
    public string? Format { get; set; }
    
    [JsonPropertyName("profiles")]
    public string? Profiles { get; set; }
    
    [JsonPropertyName("properties")]
    public Dictionary<string, string>? Properties { get; set; }
    
    [JsonPropertyName("public")]
    public bool? Public { get; set; }
    
    [JsonPropertyName("source")]
    public ImagesPostSource? Source { get; set; }
}

public sealed record ImagesPostSource
{
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }
    
    [JsonPropertyName("certificate")]
    public string? Certificate { init; get; }
    
    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { init; get; }
    
    [JsonPropertyName("image_type")]
    [JsonConverter(typeof(StringToEnumConverter<ImageType>))]
    public ImageType? ImageType { init; get; }
    
    [JsonPropertyName("mode")]
    [JsonConverter(typeof(StringToEnumConverter<TransferMode>))]
    public TransferMode? Mode { init; get; }
    
    [JsonPropertyName("project")]
    public string? Project { init; get; }
    
    [JsonPropertyName("protocol")]
    public string? Protocol { init; get; }
    
    [JsonPropertyName("secret")]
    public string? Secret { init; get; }
    
    [JsonPropertyName("server")]
    public string? Server { init; get; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(StringToEnumConverter<ImageSourceType>))]
    public ImageSourceType? Type { init; get; }
    
    [JsonPropertyName("url")]
    public string? Url { init; get; }
}