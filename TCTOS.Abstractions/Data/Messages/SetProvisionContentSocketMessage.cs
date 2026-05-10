using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record SetProvisionContentSocketMessage() : SocketMessage(SocketMessageTypes.SetProvisioningContent)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
    
    [JsonPropertyName("content")]
    public required string Content { init; get; }
}