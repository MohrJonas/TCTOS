namespace TCTOS.Console.Commands.Container.Feature;

public sealed class FeatureSubSubCommand : CommandBase
{
    public FeatureSubSubCommand()
        : base("feature", "Container feature-related commands")
    {
        Subcommands.Add(new AddFeatureCommand());
        Subcommands.Add(new RemoveFeatureCommand());
        //Subcommands.Add(new CheckFeaturesCommand());
    }
}