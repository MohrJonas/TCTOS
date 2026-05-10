using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record ListContainerFeaturesSocketMessage() : SocketMessage(SocketMessageTypes.ListContainers)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
}