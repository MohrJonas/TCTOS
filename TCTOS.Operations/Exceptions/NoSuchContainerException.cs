namespace TCTOS.Operations.Exceptions;

public sealed class NoSuchContainerException(string containerName)
    : Exception($"Container \"{containerName}\" does not exist");