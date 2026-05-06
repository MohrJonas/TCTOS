namespace TCTOS.Abstractions.Data.DesktopFiles;

public sealed record HeaderDesktopFileLine(string HeaderValue) : IDesktopFileLine
{
    public string SerializeToString()
        => $"[{HeaderValue}]";
}