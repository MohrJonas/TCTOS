using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.Devices;

public sealed record IncusDiskDevice : IIncusDevice
{
    [JsonPropertyName("path")] public required string Path { get; set; }

    [JsonPropertyName("pool")] public string? Pool { get; set; }

    [JsonPropertyName("shift")] public bool? Shift { init; get; }

    [JsonPropertyName("source")] public string? Source { init; get; }

    [JsonPropertyName("type")] public string Type => "disk";
}