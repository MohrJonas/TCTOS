using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record GetProvisionContentSocketMessage() : SocketMessage(SocketMessageTypes.GetProvisioningContent)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
}