namespace TCTOS.Console.Commands.Container.Provision;

public sealed class ProvisionSubSubCommand : CommandBase
{
    public ProvisionSubSubCommand()
        : base("provision", "Container provision-related commands", ["prov"])
    {
        Subcommands.Add(new ProvisionContainerCommand());
        Subcommands.Add(new EditContainerProvisionCommand());
        Subcommands.Add(new ShowContainerProvisionCommand());
    }
}