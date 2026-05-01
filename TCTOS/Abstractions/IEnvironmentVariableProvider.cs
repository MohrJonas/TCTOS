namespace TCTOS.Abstractions;

public interface IEnvironmentVariableProvider
{
    public bool HasVariable(string name);
    public string GetVariableValue(string name);
}