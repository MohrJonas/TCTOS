using System.CommandLine;

namespace TCTOS.Console.Commands;

public abstract class CommandBase : Command
{
    protected CommandBase(string name, string description, DiContainer container, string[]? aliases = null,
        Option[]? options = null, Argument[]? arguments = null)
        : base(name, description)
    {
        foreach (var alias in aliases ?? [])
            Aliases.Add(alias);
        foreach (var option in options ?? [])
            Options.Add(option);
        foreach (var argument in arguments ?? [])
            Arguments.Add(argument);
        SetAction(async (result, ct) => await RunAsync(result, container, ct));
    }

    protected virtual Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}