using System.Text.Json.Serialization;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record ImageAlias
{
    [JsonPropertyName("description")]
    public string? Description { init; get; }
    
    [JsonPropertyName("name")]
    public string? Name { init; get; }
}