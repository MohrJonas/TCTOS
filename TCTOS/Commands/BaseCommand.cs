using System.CommandLine;
using TCTOS.Commands.Container;
using TCTOS.IOC;

namespace TCTOS.Commands;

public sealed class BaseCommand : RootCommand
{
    public BaseCommand(DiContainer container) : base("TheConTainerOS")
    {
        Options.Add(SharedOptions.PlainOption);
        Options.Add(SharedOptions.VerboseOption);
        Subcommands.Add(new ContainerSubCommand(container));
    }
}