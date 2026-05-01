using System.CommandLine;
using TCTOS.IOC;

namespace TCTOS.Commands.Container.Provision;

public sealed class ProvisionSubSubCommand : CommandBase
{
    public ProvisionSubSubCommand(DiContainer container) 
        : base("provision", "Container provision-related commands", container, ["prov"])
    {
        Subcommands.Add(new ProvisionContainerCommand(container));
        Subcommands.Add(new EditContainerProvisionCommand(container));
        Subcommands.Add(new ShowContainerProvisionCommand(container));
    }
}