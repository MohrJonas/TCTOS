namespace TCTOS.Operations;

public static class NameMangler
{
    public static string MangleContainerName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        // Drop leading and trailing hyphens
        var chars = name
            .Trim('-')
            .ToCharArray();
        return string.Join(string.Empty, chars.Where(static @char =>
            char.IsAsciiLetter(@char)
            || char.IsAsciiDigit(@char)
            || @char == '-'
        ));
    }
}