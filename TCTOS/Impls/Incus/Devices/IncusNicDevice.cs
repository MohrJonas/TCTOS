using System.Text.Json.Serialization;

namespace TCTOS.Impls.Incus.Devices;

public sealed record IncusNicDevice : IIncusDevice
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("network")]
    public string? Network { get; set; }
    
    [JsonPropertyName("type")] 
    public string Type => "nic";
}