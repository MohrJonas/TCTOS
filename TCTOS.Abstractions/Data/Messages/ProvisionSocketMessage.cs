using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record ProvisionSocketMessage() : SocketMessage(SocketMessageTypes.ProvisionContainer)
{
    [JsonPropertyName("containerName")] public required string ContainerName { init; get; }
    
    [JsonPropertyName("uid")] public required uint Uid { init; get; }

    [JsonPropertyName("gid")] public required uint Gid { init; get; }
}