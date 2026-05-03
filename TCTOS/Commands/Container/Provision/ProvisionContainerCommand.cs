using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.IOC;

namespace TCTOS.Commands.Container.Provision;

public sealed class ProvisionContainerCommand(DiContainer container)
    : CommandBase("run", "Provision the container via its configuration", container,
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var provisioner = container.Get<IContainerProvisioner>();
        var fileSystem = container.Get<IFileSystem>();
        var userInformationCollector = container.Get<IUserInformationCollector>();

        var provisionFileContent = (await fileSystem.GetProvisioningFileContentAsync(containerName)).GetOrThrow()!;

        Dictionary<string, string> variables = new()
        {
            { "TCTOS_GID", userInformationCollector.GetGid().ToString() },
            { "TCTOS_UID", userInformationCollector.GetUid().ToString() },
            { "ansible_python_interpreter", "auto_silent" }
        };

        (await provisioner.ProvisionContainer(containerName, provisionFileContent, variables)).ThrowIfFailed();
    }
}