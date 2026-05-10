using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record DeleteContainerSocketMessage(
    [property: JsonPropertyName("containerName")] string ContainerName
) : SocketMessage(SocketMessageTypes.DeleteContainer);