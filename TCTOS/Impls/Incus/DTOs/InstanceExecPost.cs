using System.Text.Json.Serialization;

namespace TCTOS.Impls.Incus.DTOs;

public sealed record InstanceExecPost
{
    [JsonPropertyName("command")] public required string[] Command { get; set; }

    [JsonPropertyName("environment")] public Dictionary<string, string>? Environment { get; set; }

    [JsonPropertyName("cwd")] public string? Cwd { get; set; }

    [JsonPropertyName("group")] public int? Group { get; set; }

    [JsonPropertyName("user")] public int? User { get; set; }

    [JsonPropertyName("wait-for-websocket")]
    public bool WaitForWebsocket { get; set; } = false;

    [JsonPropertyName("interactive")] public bool Interactive { get; set; } = false;
}