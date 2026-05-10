using TCTOS.Console.Commands;

namespace TCTOS.Console;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var command = new BaseCommand();

        var parseResult = command.Parse(args);
        foreach (var error in parseResult.Errors)
            await System.Console.Error.WriteLineAsync(error.Message);
        if (parseResult.Errors.Count != 0)
            Environment.Exit(1);

        return await parseResult.InvokeAsync();
    }
}