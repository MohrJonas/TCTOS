using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.IOC;

namespace TCTOS.Commands.Container.Provision;

public sealed class EditContainerProvisionCommand(DiContainer container)
    : CommandBase("edit", "Edit the container's provision file", container, arguments: [SharedArguments.ContainerNameArgument])
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
        var editorCommand = envProvider.HasVariable("EDITOR") ? envProvider.GetVariableValue("EDITOR") : "vi";
        var runner = container.Get<ICommandRunner>();
        var result = await runner.RunCommandInteractively(editorCommand, [tempFile]);
        result.ThrowIfFailed();

        (await fileSystem.SetProvisioningFileContentAsync(
            containerName, 
            await File.ReadAllTextAsync(tempFile, token)
        )).ThrowIfFailed();

        File.Delete(tempFile);
    }
}