using System.Text.Json.Serialization;
using TCTOS.Impls.Incus.Devices;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record InstancePut
{
    [JsonPropertyName("architecture")]
    public string? Architecture { get; set; }
    
    [JsonPropertyName("config")]
    public Dictionary<string, object>? Config { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("devices")]
    public Dictionary<string, object>? Devices { get; set; }
    
    [JsonPropertyName("disk_only")]
    public bool? DiskOnly { get; set; }
    
    [JsonPropertyName("ephemeral")]
    public bool? Ephemeral { get; set; }
    
    [JsonPropertyName("profiles")]
    public string[]? Profiles { get; set; }
    
    [JsonPropertyName("restore")]
    public string? Restore { get; set; }
    
    [JsonPropertyName("stateful")]
    public bool? Stateful { get; set; }
}