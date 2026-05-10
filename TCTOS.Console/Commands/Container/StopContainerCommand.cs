using System.CommandLine;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Container;

public sealed class StopContainerCommand()
    : CommandBase("stop", "Stop the container", ["halt", "down"],
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var plain = parseResult.GetRequiredValue(SharedOptions.PlainOption);
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var task = writer.WriteAsync(new StopContainerSocketMessage
        {
            ContainerName = containerName
        });
        
        var response = plain
            ? await task
            : await Spectre.Console.SpinnerExtensions.Spinner(task);
        
        response.ExitOnError();
    }
}