using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record CreateContainerSocketMessage() : SocketMessage(SocketMessageTypes.CreateContainer)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
    
    [JsonPropertyName("description")]
    public string? Description { init; get; }
    
    [JsonPropertyName("features")]
    public required string[] Features { init; get; }
    
    [JsonPropertyName("image")]
    public required string Image { init; get; }
    
    [JsonPropertyName("color")]
    public required Color Color { init; get; }
}