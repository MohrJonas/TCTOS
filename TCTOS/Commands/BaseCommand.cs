using System.CommandLine;
using TCTOS.Commands.Container;
using TCTOS.Commands.Feature;
using TCTOS.IOC;

namespace TCTOS.Commands;

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