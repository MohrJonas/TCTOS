using Spectre.Console;
using TCTOS.Abstractions.Data;

namespace TCTOS.Console;

public static class Extensions
{
    public static void ExitWithError(this string? error)
    {
        if(error == null)
            return;
        AnsiConsole.MarkupLine($"[bold red]{Markup.Escape(error)}[/]");
        Environment.Exit(1);
    }
    
    public static void ExitOnError(this SocketResponse response) => ExitWithError(response.Error);

    public static void ExitOnError<TData>(this SocketResponse<TData> response) => ExitWithError(response.Error);
}