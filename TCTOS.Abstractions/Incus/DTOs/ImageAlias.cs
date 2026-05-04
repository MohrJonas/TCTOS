using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.DTOs;

public sealed record ImageAlias
{
    [JsonPropertyName("description")] public string? Description { init; get; }

    [JsonPropertyName("name")] public string? Name { init; get; }
}