using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.IOC;

namespace TCTOS.Commands;

public static class LaunchCommandArguments
{
    public static readonly Argument<string> ExecutableNameArgument = new("executable")
    {
        Description = "The executable to run"
    };
}

public sealed class LaunchCommand(DiContainer container)
    : CommandBase("launch", "Launch the executable in the specified container", container,
        arguments: [SharedArguments.ContainerNameArgument, LaunchCommandArguments.ExecutableNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var incusClient = container.Get<IIncusClient>();

        var containers = (await incusClient.GetContainersAsync()).Metadata;

        if (containers.All(ct => ct.Name != containerName))
            throw new Exception($"Container \"{containerName}\" does not exist");

        var instance = containers.Single(ct => ct.Name == containerName);

        var userInformationCollector = container.Get<IUserInformationCollector>();
        var nonPersistentStorage = container.Get<INonPersistentStorage>();

        if (instance.Status == "Stopped")
        {
            var startResponse = (await incusClient.StartContainerAsync(containerName));
            startResponse.ThrowOnError();
            await incusClient.WaitForOperationAsync(startResponse.Operation!);
            
            var featureProvider = container.Get<IFeatureProvider>();
            var featureRunner = container.Get<IFeatureRunner>();
            var fileSystem = container.Get<IFileSystem>();

            var availableFeatures = (await featureProvider.GetAvailableFeaturesAsync()).GetOrThrow();
            var instanceConfig = (await fileSystem.GetContainerConfigurationAsync(instance.Name)).GetOrThrow();

            List<string> enabledFeatures = [];
            Dictionary<string, string> env = [];

            foreach (var featureName in instanceConfig!.FeatureNames)
            {
                if (availableFeatures.All(f => f.Name != featureName))
                    throw new Exception($"Feature with name \"{featureName}\" does not exist");

                var featureText = (await featureProvider.GetFeatureScriptTextAsync(featureName)).GetOrThrow();

                var variableProvider = container.Get<IEnvironmentVariableProvider>();
                var runner = container.Get<ICommandRunner>();
                var backgroundRunner = container.Get<IBackgroundCommandRunner>();

                if (!await featureRunner.CanApplyFeature(
                        featureText,
                        containerName,
                        fileSystem,
                        incusClient,
                        nonPersistentStorage,
                        userInformationCollector,
                        variableProvider,
                        runner,
                        backgroundRunner
                    )) continue;
                {
                    await featureRunner.ApplyFeature(
                        featureText,
                        containerName,
                        fileSystem,
                        incusClient,
                        nonPersistentStorage,
                        userInformationCollector,
                        variableProvider,
                        runner,
                        backgroundRunner,
                        env
                    );
                    enabledFeatures.Add(featureName);
                }
            }

            var key = $"{containerName}-enabled-features";
            nonPersistentStorage.PutValue(key, enabledFeatures);

            var _envKey = $"{container}-env";
            nonPersistentStorage.PutValue(_envKey, env);
        }

        var envKey = $"{container}-env";
        (await incusClient.RunCommand(containerName, "/sbin/simlog",
            [
                "--uid", userInformationCollector.GetUid().ToString(),
                "--gid", userInformationCollector.GetGid().ToString(),
                "--",
                parseResult.GetRequiredValue(LaunchCommandArguments.ExecutableNameArgument),
                ..parseResult.UnmatchedTokens
            ], nonPersistentStorage.PeekValue<Dictionary<string, object>>(envKey)
        )).ThrowOnError();
    }
}