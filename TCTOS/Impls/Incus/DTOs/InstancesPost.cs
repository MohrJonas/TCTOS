using System.Text.Json.Serialization;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record InstancesPost
{
    [JsonPropertyName("architecture")] public string? Architecture { get; set; }

    [JsonPropertyName("config")] public Dictionary<string, object>? Config { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("devices")] public Dictionary<string, object>? Devices { get; set; }

    [JsonPropertyName("disk_only")] public bool? DiskOnly { get; set; }

    [JsonPropertyName("ephemeral")] public bool? Ephemeral { get; set; }

    [JsonPropertyName("instance_type")] public string? InstanceType { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("profiles")] public string[]? Profiles { get; set; }

    [JsonPropertyName("restore")] public string? Restore { get; set; }

    [JsonPropertyName("source")] public InstanceSource? Source { get; set; }

    [JsonPropertyName("start")] public bool? Start { get; set; }

    [JsonPropertyName("stateful")] public bool? Stateful { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }
}