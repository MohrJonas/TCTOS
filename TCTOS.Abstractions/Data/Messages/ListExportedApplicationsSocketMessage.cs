using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record ListExportedApplicationsSocketMessage(
    [property: JsonPropertyName("containerName")]
    string ContainerName)
    : SocketMessage(SocketMessageTypes.ListExportedApplications)
{
}