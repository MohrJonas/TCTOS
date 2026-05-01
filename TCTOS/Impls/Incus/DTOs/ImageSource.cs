using System.Text.Json.Serialization;
using TCTOS.Impls.Incus.Conversion;
using TCTOS.Impls.Incus.Data;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record ImageSource
{
    [JsonPropertyName("alias")]
    public string? Alias { init; get; }
    
    [JsonPropertyName("certificate")]
    public string? Certificate { init; get; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(StringToEnumConverter<ImageType>))]
    public ImageType? ImageType { init; get; }
    
    [JsonPropertyName("protocol")]
    public string? Protocol { init; get; }
    
    [JsonPropertyName("server")]
    public string? Server { init; get; }
}