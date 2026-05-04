namespace TCTOS.Abstractions.Incus;

public sealed class IncusClientException(string message) : Exception(message);