using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record StopContainerSocketMessage() : SocketMessage(SocketMessageTypes.StopContainer)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
}