using TCTOS.Abstractions;

namespace TCTOS.Impls;

public sealed class LocalEnvironmentVariableProviderImpl : IEnvironmentVariableProvider
{
    public bool HasVariable(string name)
        => Environment.GetEnvironmentVariable(name) != null;

    public string GetVariableValue(string name)
        => Environment.GetEnvironmentVariable(name)!;
}