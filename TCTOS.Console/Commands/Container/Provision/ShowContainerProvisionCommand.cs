using System.CommandLine;
using System.Diagnostics;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Container.Provision;

public sealed class ShowContainerProvisionCommand()
    : CommandBase("show", "Show the container's provision file",
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var plain = parseResult.GetRequiredValue(SharedOptions.PlainOption);
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var response = await writer.WriteAsync<string>(new GetProvisionContentSocketMessage
        {
            ContainerName = containerName
        });
        
        response.ExitOnError();

        if (plain)
        {
            System.Console.Write(response.Data!);
            return;
        }
        
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, response.Data, token);

        var startInfo = new ProcessStartInfo
        {
            FileName = "bat",
            Arguments = string.Join(" ", ["-f", "-s", "--file-name", containerName, "-l", "yaml", tempFile])
        };

        try
        {
            var process = Process.Start(startInfo);
            await process?.WaitForExitAsync(token)!;
        }
        catch (Exception e)
        {
            e.Message.ExitWithError();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}