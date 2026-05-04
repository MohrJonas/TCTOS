using System.CommandLine;
using TCTOS.Console.Abstractions;
using TCTOS.Console.IOC;
using TCTOS.Console.Exceptions;

namespace TCTOS.Console.Commands.Container;

public sealed class StartContainerCommand(DiContainer container)
    : CommandBase("start", "Start the container", container, ["up"], arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var client = container.Get<IIncusClient>();

        var containerNames = (await client.GetContainerNamesAsync()).Metadata.Select(name => name.Split("/").Last());
        if (!containerNames.Contains(containerName))
            throw new NoSuchContainerException(containerName);

        var startResponse = (await client.StartContainerAsync(containerName));
        startResponse.ThrowOnError();
        await client.WaitForOperationAsync(startResponse.Operation!);

        var userInformationCollector = container.Get<IUserInformationCollector>();
        
        (await client.RunCommand(containerName, "/sbin/simlog", [
            "--uid", userInformationCollector.GetUid().ToString(),
            "--gid", userInformationCollector.GetGid().ToString(),
            "true"
        ])).ThrowOnError();

        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();

        var availableFeatures = (await featureProvider.GetAvailableFeaturesAsync()).GetOrThrow();
        var containerConfiguration = (await fileSystem.GetContainerConfigurationAsync(containerName)).GetOrThrow()!;

        var envProvider = container.Get<IEnvironmentVariableProvider>();
        var commandRunner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();

        List<string> enabledFeatures = [];
        Dictionary<string, string> env = [];

        foreach (var featureName in containerConfiguration.FeatureNames)
        {
            if (availableFeatures.All(descriptor => descriptor.Name != featureName))
                throw new NoSuchFeatureException(featureName);

            var featureText = (await featureProvider.GetFeatureScriptTextAsync(featureName)).GetOrThrow();
            if (!await featureRunner.CanApplyFeature(
                    featureText,
                    containerName,
                    fileSystem,
                    client,
                    nonPersistentStorage,
                    userInformationCollector,
                    envProvider,
                    commandRunner,
                    backgroundRunner
                )) continue;
            await featureRunner.ApplyFeature(
                featureText,
                containerName,
                fileSystem,
                client,
                nonPersistentStorage,
                userInformationCollector,
                envProvider,
                commandRunner,
                backgroundRunner,
                env
            );
            enabledFeatures.Add(featureName);
        }

        var key = $"{containerName}-enabled-features";
        nonPersistentStorage.PutValue(key, enabledFeatures);

        var envKey = $"{container}-env";
        nonPersistentStorage.PutValue(envKey, env);
    }
}