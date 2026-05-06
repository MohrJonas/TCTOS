namespace TCTOS.Impls.Local;

public static class ThemeFileParser
{
    public static Dictionary<string, Dictionary<string, string>> ParseThemeFileFromText(string text)
    {
        var lines = text.Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        Dictionary<string, Dictionary<string, string>> dict = [];

        string? currentHeader = null;
        Dictionary<string, string> values = [];

        foreach (var line in lines)
        {
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                var headerValue = line.TrimStart('[').TrimEnd(']');
                if (currentHeader != null)
                {
                    dict[currentHeader] = values;
                    values = [];
                }

                currentHeader = headerValue;
            }
            else
            {
                var parts = line.Split('=', 2);
                values[parts[0]] = parts[1];
            }
        }
        if (currentHeader != null)
            dict[currentHeader] = values;

        return dict;
    }
}