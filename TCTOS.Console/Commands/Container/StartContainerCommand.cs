using System.CommandLine;
using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Operations;

namespace TCTOS.Console.Commands.Container;

public sealed class StartContainerCommand(DiContainer container)
    : CommandBase("start", "Start the container", container, ["up"], arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var logger = container.Get<ILogger>();
        var incusClient = container.Get<IIncusClient>();
        var userInformationCollector = container.Get<IUserInformationCollector>();
        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();
        var variableProvider = container.Get<IEnvironmentVariableProvider>();
        var runner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();

        (await StartContainerOperation.StartContainerAsync(
            containerName,
            logger,
            incusClient,
            userInformationCollector,
            nonPersistentStorage,
            featureProvider,
            featureRunner,
            fileSystem,
            variableProvider,
            runner,
            backgroundRunner
        )).ThrowIfFailed();
    }
}