using TCTOS.Abstractions;

namespace TCTOS.Impls.Local;

public sealed class LocalEnvironmentVariableProviderImpl : IEnvironmentVariableProvider
{
    public bool HasVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name) != null;
    }

    public string GetVariableValue(string name)
    {
        return Environment.GetEnvironmentVariable(name)!;
    }
}