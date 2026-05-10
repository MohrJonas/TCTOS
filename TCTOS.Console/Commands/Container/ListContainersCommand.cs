using System.CommandLine;
using Spectre.Console;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Container;

public sealed class ListContainersCommand()
    : CommandBase("list", "List all containers", ["ls"])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var plain = parseResult.GetRequiredValue(SharedOptions.PlainOption);
        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var response = await writer.WriteAsync<Abstractions.Data.Container[]>(new ListContainersSocketMessage());
        
        response.ExitOnError();
        
        if(plain)
            DisplayPlain(response.Data!);
        else
            DisplayPretty(response.Data!);
    }

    private static void DisplayPlain(Abstractions.Data.Container[] instances)
    {
        foreach(var instance in instances)
            System.Console.WriteLine($"{instance.ContainerName}\t{instance.Description}\t{instance.Status}\t{string.Join(", ", instance.EnabledFeatures)}");
    }

    private static void DisplayPretty(Abstractions.Data.Container[] instances)
    {
        var table = new Table()
            .AddColumn("Name")
            .AddColumn("Description")
            .AddColumn("Status")
            .AddColumn("Features");
        foreach (var instance in instances)
            table.AddRow(instance.ContainerName, instance.Description ?? string.Empty, instance.Status, string.Join(", ", instance.EnabledFeatures));
        AnsiConsole.Write(table);
    }
}