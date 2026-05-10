using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record ListExportableApplicationsMessage() : SocketMessage(SocketMessageTypes.ListExportableApplications)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
}