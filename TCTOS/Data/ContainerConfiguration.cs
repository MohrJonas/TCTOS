using System.Text.Json.Serialization;

namespace TCTOS.Data;

public sealed record ContainerConfiguration
{
    [JsonPropertyName("features")]
    public required string[] FeatureNames { get; set; }
}