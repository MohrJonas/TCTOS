using System.CommandLine;
using TCTOS.Console.Abstractions;
using TCTOS.Console.IOC;

namespace TCTOS.Console.Commands.Feature;

public sealed class FeaturesCommand(DiContainer container)
    : CommandBase("features", "List all features", container)
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var featureProvider = container.Get<IFeatureProvider>();
        foreach (var featureDescriptor in (await featureProvider.GetAvailableFeaturesAsync()).GetOrThrow())
            System.Console.WriteLine($"{featureDescriptor.Name}\t\t{featureDescriptor.Description}");
    }
}