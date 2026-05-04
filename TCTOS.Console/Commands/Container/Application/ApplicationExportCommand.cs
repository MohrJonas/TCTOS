using System.CommandLine;
using System.Text.RegularExpressions;
using TCTOS.Console.Abstractions;
using TCTOS.Console.IOC;

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

        (await incusFileSystem.PrepareFileSystem(containerName)).ThrowIfFailed();

        var userInformationCollector = container.Get<IUserInformationCollector>();

        if (!(await incusFileSystem.DoesFileExistAsync(desktopFilePath)).GetOrThrow())
            throw new Exception($"Path {desktopFilePath} does not exist");
        try
        {
            var fileContents = (await incusFileSystem.GetFileTextAsync(desktopFilePath)).GetOrThrow();
            var patchedFileContents =
                new Regex(@"^Exec=(.*)$", RegexOptions.Multiline).Replace(fileContents, match => $"Exec={BuildExecute(
                    containerName,
                    userInformationCollector.GetUid(),
                    userInformationCollector.GetGid(),
                    match.Captures[0].Value
                )}");
            patchedFileContents =
                new Regex(@"^TryExec=(.*)$", RegexOptions.Multiline).Replace(patchedFileContents, match =>
                    $"TryExec={BuildExecute(
                        containerName,
                        userInformationCollector.GetUid(),
                        userInformationCollector.GetGid(),
                        match.Captures[1].Value
                    )}", 1);
            System.Console.WriteLine(patchedFileContents);
        }
        finally
        {
            (await incusFileSystem.DisposeFileSystem(containerName)).ThrowIfFailed();
        }
    }

    private static string BuildExecute(string containerName, uint uid, uint gid, string originalExecutable)
    {
        return $"incus exec {containerName} -- /sbin/simlog --uid {uid} --gid {gid} {originalExecutable}";
    }
}