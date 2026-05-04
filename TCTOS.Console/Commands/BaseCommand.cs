using System.CommandLine;
using TCTOS.Console.Commands.Container;
using TCTOS.Console.Commands.Feature;
using TCTOS.Console.IOC;

namespace TCTOS.Console.Commands;

public sealed class BaseCommand : RootCommand
{
    public BaseCommand(DiContainer container) : base("TheConTainerOS")
    {
        Options.Add(SharedOptions.PlainOption);
        Options.Add(SharedOptions.VerboseOption);
        Subcommands.Add(new ContainerSubCommand(container));
        Subcommands.Add(new EventsCommand(container));
        Subcommands.Add(new FeaturesCommand(container));
        Subcommands.Add(new LaunchCommand(container));
    }
}