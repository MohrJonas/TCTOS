using System.CommandLine;
using TCTOS.Commands.Container.Feature;
using TCTOS.Commands.Container.Provision;
using TCTOS.IOC;

namespace TCTOS.Commands.Container;

public sealed class ContainerSubCommand : Command
{
    public ContainerSubCommand(DiContainer container) : base("container", "Container-related operations")
    {
        Aliases.Add("ct");
        Subcommands.Add(new ProvisionSubSubCommand(container));
        Subcommands.Add(new FeatureSubSubCommand(container));
        Subcommands.Add(new ListContainersCommand(container));
        Subcommands.Add(new StartContainerCommand(container));
        Subcommands.Add(new StopContainerCommand(container));
        Subcommands.Add(new CreateContainerCommand(container));
    }
}