using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.Exceptions;
using TCTOS.IOC;

namespace TCTOS.Commands.Container;

public sealed class StopContainerCommand(DiContainer container)
    : CommandBase("stop", "Stop the container", container, ["halt", "down"],
        arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var client = container.Get<IIncusClient>();

        var containerNames = (await client.GetContainerNamesAsync()).Metadata.Select(name => name.Split("/").Last());
        if (!containerNames.Contains(containerName))
            throw new NoSuchContainerException(containerName);

        var stopResponse = (await client.StopContainerAsync(containerName));
        stopResponse.ThrowOnError();
        await client.WaitForOperationAsync(stopResponse.Operation!);

        var nonPersistentStorage = container.Get<INonPersistentStorage>();

        var key = $"{containerName}-enabled-features";
        var enabledFeatures = nonPersistentStorage.GetValue<string[]>(key);

        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();
        var userInformationCollector = container.Get<IUserInformationCollector>();
        var envProvider = container.Get<IEnvironmentVariableProvider>();
        var commandRunner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();

        foreach (var featureName in enabledFeatures)
        {
            var featureText = (await featureProvider.GetFeatureScriptTextAsync(featureName)).GetOrThrow();
            await featureRunner.UnapplyFeature(
                featureText,
                containerName,
                fileSystem,
                client,
                nonPersistentStorage,
                userInformationCollector,
                envProvider,
                commandRunner,
                backgroundRunner
            );
        }
    }
}