namespace TCTOS.Abstractions.Data.DesktopFiles;

public sealed record DesktopFile(IDesktopFileLine[] Lines)
{
    public string SerializeToString() 
        => string.Join("\n", Lines.Select(l => l.SerializeToString()));
}