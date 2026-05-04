namespace TCTOS.Operations.Exceptions;

public sealed class ContainerAlreadyExistsException(string containerName)
    : Exception($"A container with name \"{containerName}\" already exists")
{
}