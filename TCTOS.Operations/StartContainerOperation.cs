using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Operations;

public static class StartContainerOperation
{
    public static Task<Result> StartContainerAsync(
        string containerName,
        ILogger logger,
        IIncusClient incusClient,
        IUserInformationCollector userInformationCollector,
        INonPersistentStorage nonPersistentStorage,
        IFeatureProvider featureProvider,
        IFeatureRunner featureRunner,
        IFileSystem fileSystem,
        IEnvironmentVariableProvider environmentVariableProvider,
        ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner
    ) =>
        RunCatchingAsync(async () =>
        {
            var containerNames =
                (await incusClient.GetContainerNamesAsync()).Metadata.Select(name => name.Split("/").Last());
            if (!containerNames.Contains(containerName))
                throw new NoSuchContainerException(containerName);

            var instanceResponse = (await incusClient.GetContainerAsync(containerName));
            instanceResponse.ThrowOnError();
            var instance = instanceResponse.Metadata;

            if (instance.Status == "Started")
                return;
            
            var startResponse = (await incusClient.StartContainerAsync(containerName));
            startResponse.ThrowOnError();
            await incusClient.WaitForOperationAsync(startResponse.Operation!);

            (await incusClient.RunCommand(containerName, "/sbin/simlog", [
                "--uid", userInformationCollector.GetUid().ToString(),
                "--gid", userInformationCollector.GetGid().ToString(),
                "true"
            ])).ThrowOnError();

            var availableFeatures = (await featureProvider.GetAvailableFeaturesAsync()).GetOrThrow();
            var containerConfiguration = (await fileSystem.GetContainerConfigurationAsync(containerName)).GetOrThrow()!;

            List<string> enabledFeatures = [];
            Dictionary<string, string> env = [];

            foreach (var featureName in containerConfiguration.FeatureNames)
            {
                if (availableFeatures.All(descriptor => descriptor.Name != featureName))
                    throw new NoSuchFeatureException(featureName);

                var featureText = (await featureProvider.GetFeatureScriptTextAsync(featureName)).GetOrThrow();
                var canApplyResult = await featureRunner.CanApplyFeature(
                    featureText,
                    containerName,
                    fileSystem,
                    incusClient,
                    nonPersistentStorage,
                    userInformationCollector,
                    environmentVariableProvider,
                    commandRunner,
                    backgroundCommandRunner
                );
                if (!canApplyResult.Data)
                {
                    logger.LogWarning("Skipping result {featureName}, because {reason}", featureName,
                        canApplyResult.Explanation);
                    continue;
                }
                (await featureRunner.ApplyFeature(
                    featureText,
                    containerName,
                    fileSystem,
                    incusClient,
                    nonPersistentStorage,
                    userInformationCollector,
                    environmentVariableProvider,
                    commandRunner,
                    backgroundCommandRunner,
                    env
                )).ThrowIfFailed();
                enabledFeatures.Add(featureName);
            }

            var key = $"{containerName}-enabled-features";
            nonPersistentStorage.PutValue(key, enabledFeatures);

            var envKey = $"{containerName}-env";
            nonPersistentStorage.PutValue(envKey, env);
        });
}