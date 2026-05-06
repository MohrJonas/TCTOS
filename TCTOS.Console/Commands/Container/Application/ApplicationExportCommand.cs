using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.Operations;

namespace TCTOS.Console.Commands.Container.Application;

public static class ApplicationExportCommandArguments
{
    public static readonly Argument<string> DesktopFilePath = new("desktop_file_path")
    {
        Description = "The path in the container to the desktop file"
    };
}

public sealed class ApplicationExportCommand(DiContainer container)
    : CommandBase("export", "Export the .desktop onto the host system", container,
        arguments: [SharedArguments.ContainerNameArgument, ApplicationExportCommandArguments.DesktopFilePath])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var desktopFilePath = parseResult.GetRequiredValue(ApplicationExportCommandArguments.DesktopFilePath);
        
        var incusFileSystem = container.Get<IIncusFileSystem>();
        var computer = container.Get<IComputer>();

        (await ExportApplicationOperation.ExportApplicationAsync(containerName, desktopFilePath, computer,
            incusFileSystem)).ThrowIfFailed();
    }

    private static string BuildExecute(string containerName, uint uid, uint gid, string originalExecutable)
    {
        return $"incus exec {containerName} -- /sbin/simlog --uid {uid} --gid {gid} {originalExecutable}";
    }
}