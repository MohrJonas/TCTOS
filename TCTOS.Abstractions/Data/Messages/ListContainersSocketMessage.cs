namespace TCTOS.Abstractions.Data.Messages;

public sealed record ListContainersSocketMessage() : SocketMessage(SocketMessageTypes.ListContainers);