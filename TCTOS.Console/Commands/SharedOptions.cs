using System.CommandLine;

namespace TCTOS.Console.Commands;

public static class SharedOptions
{
    public static readonly Option<bool> VerboseOption = new("--verbose", "-v")
    {
        Description = "Enable verbose output",
        DefaultValueFactory = _ => false
    };

    public static readonly Option<bool> PlainOption = new("--plain", "-p")
    {
        Description = "Use plain output format, stripping all colors, progress bars etc.",
        DefaultValueFactory = _ => false
    };

    public static readonly Option<string> SocketPathOption = new("--socket")
    {
        Description = "Path to the tctos control socket",
        DefaultValueFactory = _ => "/var/lib/tctos/tctos.socket"
    };
}