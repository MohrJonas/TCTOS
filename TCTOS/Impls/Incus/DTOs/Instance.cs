using System.Text.Json.Serialization;
using TCTOS.Impls.Incus.Devices;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record Instance
{
    [JsonPropertyName("architecture")]
    public required string Architecture { init; get; }
    
    [JsonPropertyName("config")]
    public required Dictionary<string, object> Config { init; get; }
    
    [JsonPropertyName("created_at")]
    public required DateTime CreatedAt { init; get; }
    
    [JsonPropertyName("description")]
    public required string Description { init; get; }
    
    [JsonPropertyName("devices")]
    public required Dictionary<string, object> Devices { init; get; }
    
    [JsonPropertyName("ephemeral")]
    public required bool Ephemeral { init; get; }
    
    [JsonPropertyName("expanded_config")]
    public required Dictionary<string, object> ExpandedConfig { init; get; }
    
    [JsonPropertyName("expanded_devices")]
    public required Dictionary<string, object> ExpandedDevices { init; get; }
    
    [JsonPropertyName("last_used_at")]
    public required DateTime LastUsedAt { init; get; }
    
    [JsonPropertyName("location")]
    public required string Location { init; get; }
    
    [JsonPropertyName("name")]
    public required string Name { init; get; }
    
    [JsonPropertyName("profiles")]
    public required string[] Profiles { init; get; }
    
    [JsonPropertyName("project")]
    public required string Project { init; get; }
    
    [JsonPropertyName("stateful")]
    public required bool Stateful { init; get; }
    
    [JsonPropertyName("status")]
    public required string Status { init; get; }
    
    [JsonPropertyName("status_code")]
    public required int StatusCode { init; get; }
    
    [JsonPropertyName("type")]
    public required string Type { init; get; }
}