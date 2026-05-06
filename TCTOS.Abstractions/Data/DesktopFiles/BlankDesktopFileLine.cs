namespace TCTOS.Abstractions.Data.DesktopFiles;

public sealed record BlankDesktopFileLine : IDesktopFileLine
{
    public string SerializeToString()
        => string.Empty;
}