using System.CommandLine;
using TCTOS.Console.Commands.Container.Application;
using TCTOS.Console.Commands.Container.Feature;
using TCTOS.Console.Commands.Container.Provision;

namespace TCTOS.Console.Commands.Container;

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
        Subcommands.Add(new ApplicationSubSubCommand(container));
    }
}