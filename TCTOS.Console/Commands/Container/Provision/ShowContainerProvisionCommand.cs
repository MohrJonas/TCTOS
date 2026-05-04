using System.CommandLine;
using TCTOS.Console.Abstractions;
using TCTOS.Console.IOC;

namespace TCTOS.Console.Commands.Container.Provision;

public sealed class ShowContainerProvisionCommand(DiContainer container)
    : CommandBase("show", "Show the container's provision file", container,
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var runner = container.Get<ICommandRunner>();

        var tempFile = Path.GetTempFileName();
        var fileSystem = container.Get<IFileSystem>();

        var contents = (await fileSystem.GetProvisioningFileContentAsync(containerName)).GetOrThrow();

        if (parseResult.RootCommandResult.GetRequiredValue(SharedOptions.PlainOption))
        {
            System.Console.WriteLine(contents);
            return;
        }

        await File.WriteAllTextAsync(tempFile, contents, token);

        var result = await runner.RunCommandInteractively(
            "bat",
            // Here we hard-code yaml syntax highlighting, even though theoretically speaking the file could be any language
            ["-f", "-s", "--file-name", containerName, "-l", "yaml", tempFile]
        );

        result.ThrowIfFailed();

        File.Delete(tempFile);
    }
}