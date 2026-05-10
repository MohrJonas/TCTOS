using System.CommandLine;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;
using TCTOS.Impls.Local;

namespace TCTOS.Console.Commands.Container;

public sealed class StartContainerCommand()
    : CommandBase("start", "Start the container", ["up"], arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var plain = parseResult.GetRequiredValue(SharedOptions.PlainOption);
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var userInformationCollector = new LocalUserInformationCollector();
        
        var task = writer.WriteAsync(new StartContainerSocketMessage
        {
            ContainerName = containerName,
            Gid = userInformationCollector.GetGid(),
            Uid = userInformationCollector.GetUid()
        });
        
        var response = plain
            ? await task
            : await Spectre.Console.SpinnerExtensions.Spinner(task);
        
        response.ExitOnError();

    }
}