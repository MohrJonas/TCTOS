using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.Devices;

public sealed record IncusNicDevice : IIncusDevice
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("network")] public string? Network { get; set; }

    [JsonPropertyName("type")] public string Type => "nic";
}