using System.CommandLine;
using TCTOS.Console.Abstractions;
using TCTOS.Console.IOC;

namespace TCTOS.Console.Commands.Container.Application;

public sealed class ApplicationListCommand(DiContainer container)
    : CommandBase("list", "List all applications in the container that can be exported", container, ["ls"],
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var incusFileSystem = container.Get<IIncusFileSystem>();

        (await incusFileSystem.PrepareFileSystem(containerName)).ThrowIfFailed();

        string[] searchPaths = ["/usr/share/applications"];

        foreach (var searchPath in searchPaths)
        {
            if (!(await incusFileSystem.DoesDirectoryExistAsync(searchPath)).GetOrThrow())
                continue;
            foreach (var filePath in (await incusFileSystem.ListFilesAsync(searchPath)).GetOrThrow())
                System.Console.WriteLine(Path.GetFileNameWithoutExtension(filePath));
        }

        (await incusFileSystem.DisposeFileSystem(containerName)).ThrowIfFailed();
    }
}