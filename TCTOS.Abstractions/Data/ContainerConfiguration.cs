using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data;

public sealed record ContainerConfiguration
{
    [JsonPropertyName("features")] public required string[] FeatureNames { get; set; }

    [JsonPropertyName("color")] public required Color Color { get; set; }
}