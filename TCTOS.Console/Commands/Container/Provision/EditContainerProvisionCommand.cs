using System.CommandLine;
using System.Diagnostics;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Container.Provision;

public sealed class EditContainerProvisionCommand()
    : CommandBase("edit", "Edit the container's provision file",
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var fileContentsResponse = await writer.WriteAsync<string>(new GetProvisionContentSocketMessage
        {
            ContainerName = containerName
        });
        
        fileContentsResponse.ExitOnError();
        
        var tempFile = Path.GetTempFileName();

        await File.WriteAllTextAsync(tempFile, fileContentsResponse.Data, token);
        
        // Lets use vi as a relatively safe fallback
        var editorCommand = Environment.GetEnvironmentVariable("EDITOR") ?? "vi";

        string[]? editorArgs = null;
        
        // If the defined editor command contains arguments, we have to split them off and pass them into args
        // e.g. --wait when using VSCode
        if (editorCommand.Contains(' '))
        {
            var commandParts = editorCommand.Split(' ', StringSplitOptions.TrimEntries);
            editorCommand = commandParts[0];
            editorArgs = commandParts[1..];
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = editorCommand,
            Arguments = string.Join(" ", [..editorArgs ?? [], tempFile])
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