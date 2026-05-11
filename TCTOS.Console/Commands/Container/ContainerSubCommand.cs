using System.CommandLine;
using TCTOS.Console.Commands.Container.Application;
using TCTOS.Console.Commands.Container.Feature;
using TCTOS.Console.Commands.Container.Provision;

namespace TCTOS.Console.Commands.Container;

public sealed class ContainerSubCommand : Command
{
    public ContainerSubCommand() : base("container", "Container-related operations")
    {
        Aliases.Add("ct");
        Subcommands.Add(new ProvisionSubSubCommand());
        Subcommands.Add(new FeatureSubSubCommand());
        Subcommands.Add(new ListContainersCommand());
        Subcommands.Add(new StartContainerCommand());
        Subcommands.Add(new StopContainerCommand());
        Subcommands.Add(new CreateContainerCommand());
        Subcommands.Add(new ApplicationSubSubCommand());
        Subcommands.Add(new DeleteContainerCommand());
        Subcommands.Add(new ContainerShellCommand());
    }
}