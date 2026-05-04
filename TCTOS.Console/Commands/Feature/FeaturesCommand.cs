using System.CommandLine;
using Spectre.Console;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;

namespace TCTOS.Console.Commands.Feature;

public sealed class FeaturesCommand(DiContainer container)
    : CommandBase("features", "List all features", container)
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var featureProvider = container.Get<IFeatureProvider>();
        var featureDescriptors = (await featureProvider.GetAvailableFeaturesAsync()).GetOrThrow();
        if(parseResult.RootCommandResult.GetRequiredValue(SharedOptions.PlainOption))
            DisplayPlain(featureDescriptors);
        else
            DisplayPretty(featureDescriptors);
    }

    private static void DisplayPlain(FeatureDescriptor[] featureDescriptors)
    {
        foreach (var featureDescriptor in featureDescriptors)
            System.Console.WriteLine(
                $"{featureDescriptor.Name}\t{featureDescriptor.Description}\t{string.Join(", ", featureDescriptor.DependsOn)}\t{string.Join(", ", featureDescriptor.ConflictsWith)}");
    }

    private static void DisplayPretty(FeatureDescriptor[] featureDescriptors)
    {
        var table = new Table();
        table
            .AddColumn("Name")
            .AddColumn("Description")
            .AddColumn("Depends On")
            .AddColumn("Conflicts with");
        foreach (var featureDescriptor in featureDescriptors)
            table.AddRow(
                featureDescriptor.Name,
                featureDescriptor.Description,
                string.Join(", ", featureDescriptor.DependsOn),
                string.Join(", ", featureDescriptor.ConflictsWith)
            );
        AnsiConsole.Write(table);
    }
}