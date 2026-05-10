namespace TCTOS.Console.Commands.Container.Application;

public sealed class ApplicationSubSubCommand : CommandBase
{
    public ApplicationSubSubCommand() : base("application",
        "container application-related commands")
    {
        Subcommands.Add(new ApplicationListCommand());
        Subcommands.Add(new ApplicationExportCommand());
    }
}