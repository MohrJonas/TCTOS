using System.Text.Json.Serialization;

namespace TCTOS.Data;

public sealed record TctOsConfiguration
{
    [JsonPropertyName("pool")] public required string PoolName { get; set; }

    [JsonPropertyName("bridge")] public required string BridgeName { get; set; }
}