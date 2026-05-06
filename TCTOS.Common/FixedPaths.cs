namespace TCTOS.Common;

public static class FixedPaths
{
    public static readonly string DefaultPersistentRootPath =
        Path.Combine(Environment.GetEnvironmentVariable("HOME")!, ".config", "tctos");  
}