using TCTOS.Abstractions.Data.DesktopFiles;

namespace TCTOS.Operations;

internal static class DesktopFileParser
{
    public static DesktopFile ParseFromText(string text)
    {
        var lines = text.Split("\n", StringSplitOptions.TrimEntries);
        List<IDesktopFileLine> desktopFileLines = [];

        foreach (var line in lines)
        {
            if(line.StartsWith('#'))
                desktopFileLines.Add(new CommentDesktopFileLine(line));
            else if(string.IsNullOrWhiteSpace(line))
                desktopFileLines.Add(new BlankDesktopFileLine());
            else if(line.StartsWith('[') && line.EndsWith(']'))
                desktopFileLines.Add(new HeaderDesktopFileLine(line.TrimStart('[').TrimEnd(']')));
            else if (line.Contains('='))
            {
                var parts = line.Split("=", 2);
                desktopFileLines.Add(new KeyValueDesktopFileLine(parts[0], parts[1]));
            }
            else
                throw new Exception($"Cannot parse desktop file line \"{line}\"");
        }
        
        return new DesktopFile(desktopFileLines.ToArray());
    }
}