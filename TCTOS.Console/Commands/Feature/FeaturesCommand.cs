using System.CommandLine;
using Spectre.Console;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Feature;

public sealed class FeaturesCommand()
    : CommandBase("features", "List all features")
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var plain = parseResult.GetRequiredValue(SharedOptions.PlainOption);
        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);
        var featureResponse = await writer.WriteAsync<FeatureDescriptor[]>(new ListAllFeaturesSocketMessage());
        
        featureResponse.ExitOnError();
        
        if(plain)
            DisplayPlain(featureResponse.Data!);
        else
            DisplayPretty(featureResponse.Data!);
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