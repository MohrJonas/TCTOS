namespace TCTOS.Operations.Exceptions;

public sealed class InvalidContainerNameException(string containerName) : Exception($"Container name \"{containerName}\" is invalid");