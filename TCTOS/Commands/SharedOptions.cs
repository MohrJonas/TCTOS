using System.CommandLine;

namespace TCTOS.Commands;

public static class SharedOptions
{
    public static Option<bool> VerboseOption = new("--verbose", "-v")
    {
        Description = "Enable verbose output",
        DefaultValueFactory = _ => false
    };

    public static Option<bool> PlainOption = new("--plain", "-p")
    {
        Description = "Use plain output format, stripping all colors, progress bars etc.",
        DefaultValueFactory = _ => false
    };
}