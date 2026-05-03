using TCTOS.IOC;

namespace TCTOS.Commands.Container.Application;

public sealed class ApplicationSubSubCommand : CommandBase
{
    public ApplicationSubSubCommand(DiContainer container) : base("application",
        "container application-related commands", container)
    {
        Subcommands.Add(new ApplicationListCommand(container));
        Subcommands.Add(new ApplicationExportCommand(container));
    }
}