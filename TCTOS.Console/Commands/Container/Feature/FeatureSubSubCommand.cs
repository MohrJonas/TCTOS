using TCTOS.Console.IOC;

namespace TCTOS.Console.Commands.Container.Feature;

public sealed class FeatureSubSubCommand : CommandBase
{
    public FeatureSubSubCommand(DiContainer container)
        : base("feature", "Container feature-related commands", container)
    {
        Subcommands.Add(new ApplyFeaturesCommand(container));
        Subcommands.Add(new UnapplyFeaturesCommand(container));
        Subcommands.Add(new CheckFeaturesCommand(container));
    }
}