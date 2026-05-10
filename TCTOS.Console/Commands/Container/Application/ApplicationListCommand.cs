using System.CommandLine;
using Spectre.Console;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Container.Application;

public sealed class ApplicationListCommand()
    : CommandBase("list", "List all applications in the container that can be exported", ["ls"],
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var plain = parseResult.GetRequiredValue(SharedOptions.PlainOption);
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        
        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var response = await writer.WriteAsync<string[]>(new ListExportableApplicationsMessage
        {
            ContainerName = containerName
        });
        
        response.ExitOnError();
        
        if(plain)
            DisplayPlain(response.Data!);
        else
            DisplayPretty(response.Data!);
    }

    private static void DisplayPlain(string[] applications)
        => System.Console.Write(string.Join("\n", applications));
    
    private static void DisplayPretty(string[] applications)
    {
        var rows = new Rows(
            applications.Select(a => new Markup(Markup.Escape(a)))
        );
        AnsiConsole.Write(rows);
    }
}