using System.Text.Json.Serialization;
using TCTOS.Abstractions.Incus.Conversion;
using TCTOS.Abstractions.Incus.Data;

namespace TCTOS.Abstractions.Incus.DTOs;

public sealed record ImageSource
{
    [JsonPropertyName("alias")] public string? Alias { init; get; }

    [JsonPropertyName("certificate")] public string? Certificate { init; get; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(StringToEnumConverter<ImageType>))]
    public ImageType? ImageType { init; get; }

    [JsonPropertyName("protocol")] public string? Protocol { init; get; }

    [JsonPropertyName("server")] public string? Server { init; get; }
}