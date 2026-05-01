using System.Text.Json.Serialization;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record InstanceStatePut
{
    [JsonPropertyName("action")]
    public required string Action { init; get; }
    
    [JsonPropertyName("force")]
    public bool? Force { init; get; }
    
    [JsonPropertyName("stateful")]
    public bool? Stateful { init; get; }
    
    [JsonPropertyName("timeout")]
    public int? Timeout { init; get; }
}