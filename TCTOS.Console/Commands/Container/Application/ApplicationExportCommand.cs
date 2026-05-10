using System.CommandLine;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Container.Application;

public static class ApplicationExportCommandArguments
{
    public static readonly Argument<string> DesktopFilePath = new("desktop_file_path")
    {
        Description = "The path in the container to the desktop file"
    };
}

public sealed class ApplicationExportCommand()
    : CommandBase("export", "Export the .desktop onto the host system",
        arguments: [SharedArguments.ContainerNameArgument, ApplicationExportCommandArguments.DesktopFilePath])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var desktopFilePath = parseResult.GetRequiredValue(ApplicationExportCommandArguments.DesktopFilePath);
        
        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var response = await writer.WriteAsync<string[]>(new ExportApplicationSocketMessage
        {
            ContainerName = containerName,
            ApplicationPath = desktopFilePath
        });
        
        response.ExitOnError();   
    }
}