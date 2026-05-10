using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record ExportApplicationSocketMessage() : SocketMessage(SocketMessageTypes.ExportApplication)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
    
    [JsonPropertyName("applicationPath")]
    public required string ApplicationPath { init; get; }
}