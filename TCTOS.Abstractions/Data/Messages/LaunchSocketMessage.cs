using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record LaunchSocketMessage() : SocketMessage(SocketMessageTypes.Launch)
{
    [JsonPropertyName("uid")]
    public required uint Uid { init; get; }
    
    [JsonPropertyName("gid")]
    public required uint Gid { init; get; }
    
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
    
    [JsonPropertyName("command")]
    public required string Command { init; get; }
    
    [JsonPropertyName("args")]
    public string[]? Args { init; get; }
}