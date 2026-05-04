using System.CommandLine;
using TCTOS.Abstractions;

namespace TCTOS.Console.Commands.Container.Provision;

public sealed class EditContainerProvisionCommand(DiContainer container)
    : CommandBase("edit", "Edit the container's provision file", container,
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var tempFile = Path.GetTempFileName();
        var fileSystem = container.Get<IFileSystem>();

        await File.WriteAllTextAsync(
            tempFile,
            (await fileSystem.GetProvisioningFileContentAsync(containerName)).GetOrThrow(),
            token
        );

        var envProvider = container.Get<IEnvironmentVariableProvider>();
        
        var editorCommand = envProvider.HasVariable("EDITOR") 
            ? envProvider.GetVariableValue("EDITOR") 
            : null;

        string[]? editorArgs = null;
        
        if (editorCommand != null)
        {
            if (editorCommand.Contains(' '))
            {
                var commandParts = editorCommand.Split(' ', StringSplitOptions.TrimEntries);
                editorCommand = commandParts[0];
                editorArgs = commandParts[1..];
            }
        }

        editorCommand ??= "vi";
        editorArgs ??= [];
        
        var runner = container.Get<ICommandRunner>();
        var result = await runner.RunCommandInteractively(editorCommand, [..editorArgs, tempFile]);
        result.ThrowIfFailed();

        (await fileSystem.SetProvisioningFileContentAsync(
            containerName,
            await File.ReadAllTextAsync(tempFile, token)
        )).ThrowIfFailed();

        File.Delete(tempFile);
    }
}