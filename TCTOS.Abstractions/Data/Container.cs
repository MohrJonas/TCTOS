using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data;

public sealed record Container(
    [property:JsonPropertyName("containerName")] string ContainerName, 
    [property:JsonPropertyName("description")] string? Description, 
    [property:JsonPropertyName("status")] string Status, 
    [property:JsonPropertyName("features")] string[] EnabledFeatures
);