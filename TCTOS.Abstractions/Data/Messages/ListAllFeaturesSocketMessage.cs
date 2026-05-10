namespace TCTOS.Abstractions.Data.Messages;

public sealed record ListAllFeaturesSocketMessage() : SocketMessage(SocketMessageTypes.ListAllFeatures);