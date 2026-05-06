namespace TCTOS.Abstractions.Data.DesktopFiles;

public sealed record KeyValueDesktopFileLine(string Key, string Value) : IDesktopFileLine
{
    public string SerializeToString()
        => $"{Key}={Value}";
}