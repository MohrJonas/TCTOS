using System.CommandLine;
using Spectre.Console;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Incus.DTOs;

namespace TCTOS.Console.Commands.Container;

public sealed class ListContainersCommand(DiContainer container)
    : CommandBase("list", "List all containers", container, ["ls"])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var client = container.Get<IIncusClient>();
        
        var containersResponse = await client.GetContainersAsync();
        containersResponse.ThrowOnError();
        var containers = containersResponse.Metadata;
        if(parseResult.RootCommandResult.GetRequiredValue(SharedOptions.PlainOption))
            DisplayPlain(containers);
        else
            DisplayPretty(containers);
    }

    private static void DisplayPlain(Instance[] instances)
    {
        foreach(var instance in instances)
            System.Console.WriteLine($"{instance.Name}\t{instance.Description}\t{instance.Status},{instance.Type}");
    }

    private static void DisplayPretty(Instance[] instances)
    {
        var table = new Table()
            .AddColumn("Name")
            .AddColumn("Description")
            .AddColumn("Status")
            .AddColumn("Type");
        foreach (var instance in instances)
            table.AddRow(instance.Name, instance.Description, instance.Status, instance.Type);
        AnsiConsole.Write(table);
    }
}