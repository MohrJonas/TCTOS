namespace TCTOS.Operations;

public static class NameValidator
{
    public static bool IsValidContainerName(string name)
    {
        switch (name.Length)
        {
            case 0:
                return false;
            case 1:
                return char.IsAsciiLetterOrDigit(name[0]);
        }

        if (!char.IsAsciiLetterOrDigit(name.First()) || !char.IsAsciiLetterOrDigit(name.Last()))
            return false;
        return name.All(c => char.IsAsciiLetterOrDigit(c) || c == '-');
    }
}