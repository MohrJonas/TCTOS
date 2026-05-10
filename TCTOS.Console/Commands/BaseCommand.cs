using System.CommandLine;
using TCTOS.Console.Commands.Container;
using TCTOS.Console.Commands.Feature;

namespace TCTOS.Console.Commands;

public sealed class BaseCommand : RootCommand
{
    public BaseCommand() : base("TheContainerControlCenter")
    {
        Options.Add(SharedOptions.PlainOption);
        Options.Add(SharedOptions.VerboseOption);
        Options.Add(SharedOptions.SocketPathOption);
        Subcommands.Add(new ContainerSubCommand());
        //Subcommands.Add(new EventsCommand(container));
        Subcommands.Add(new FeaturesCommand());
        Subcommands.Add(new LaunchCommand());
        //Subcommands.Add(new InitializeOsCommand(container));
    }
}