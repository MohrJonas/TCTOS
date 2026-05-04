using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data;

public sealed record FeatureDescriptor
{
    [JsonPropertyName("name")] public required string Name { init; get; }

    [JsonPropertyName("description")] public required string Description { init; get; }

    [JsonPropertyName("conflicts")] public required string[] ConflictsWith { init; get; }

    [JsonPropertyName("depends")] public required string[] DependsOn { init; get; }
}