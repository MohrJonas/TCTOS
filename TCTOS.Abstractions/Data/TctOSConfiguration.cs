using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data;

public sealed record TctOsConfiguration
{
    [JsonPropertyName("pool")] public required string PoolName { get; set; }

    [JsonPropertyName("bridge")] public required string BridgeName { get; set; }
    
    [JsonPropertyName("nonPersistentRoot")] public required string NonPersistentRoot { get; set; }
}