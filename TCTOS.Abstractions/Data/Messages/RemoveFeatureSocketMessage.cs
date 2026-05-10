using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data.Messages;

public sealed record RemoveFeatureSocketMessage() : SocketMessage(SocketMessageTypes.RemoveFeature)
{
    [JsonPropertyName("containerName")]
    public required string ContainerName { init; get; }
    
    [JsonPropertyName("featureName")]
    public required string FeatureName { init; get; }
}