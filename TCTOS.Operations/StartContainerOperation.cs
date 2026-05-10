using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Operations;

public static class StartContainerOperation
{
    public static Task<Result> StartContainerAsync(
        string containerName,
        uint uid,
        uint gid,
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
            logger.LogInformation("Starting container operation for {containerName}", containerName);

            var containerNames =
                (await incusClient.GetContainerNamesAsync()).Metadata.Select(n => n.Split("/").Last()).ToArray();

            logger.LogDebug("Available containers: {containers}", string.Join(", ", containerNames));

            if (!containerNames.Contains(containerName))
                throw new NoSuchContainerException(containerName);

            logger.LogInformation("Fetching container state for {containerName}", containerName);
            var instanceResponse = await incusClient.GetContainerAsync(containerName);
            instanceResponse.ThrowOnError();
            var instance = instanceResponse.Metadata;

            logger.LogDebug("Container {containerName} status: {status}", containerName, instance.Status);

            if (instance.Status == "Running")
            {
                logger.LogInformation("Container {containerName} is already started", containerName);
                return;
            }

            logger.LogInformation("Starting container {containerName}", containerName);
            var startResponse = await incusClient.StartContainerAsync(containerName);
            startResponse.ThrowOnError();
            (await incusClient.WaitForOperationAsync(startResponse.Operation!)).ThrowOnError();
            logger.LogInformation("Container {containerName} started successfully", containerName);

            logger.LogInformation("Running simlog for {containerName}", containerName);
            (await incusClient.RunCommand(
                containerName,
                "/sbin/simlog",
                0, 0,
                [
                    "--uid", uid.ToString(),
                    "--gid", gid.ToString(),
                    "true"
                ]
            )).ThrowOnError();

            logger.LogInformation("Loading available features");
            var availableFeatures = (await featureProvider.GetAvailableFeaturesAsync()).GetOrThrow();
            logger.LogDebug("Available features: {features}",
                string.Join(", ", availableFeatures.Select(f => f.Name)));

            var containerConfiguration =
                (await fileSystem.GetContainerConfigurationAsync(containerName)).GetOrThrow()!;

            logger.LogInformation("Configured features for {containerName}: {features}",
                containerName, string.Join(", ", containerConfiguration.FeatureNames));

            List<string> enabledFeatures = [];
            Dictionary<string, string> env = [];

            foreach (var featureName in containerConfiguration.FeatureNames)
            {
                logger.LogDebug("Processing feature {featureName}", featureName);

                if (availableFeatures.All(d => d.Name != featureName))
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
                    logger.LogWarning(
                        "Skipping feature {featureName}, reason: {reason}",
                        featureName,
                        canApplyResult.Explanation
                    );
                    continue;
                }

                logger.LogInformation("Applying feature {featureName}", featureName);

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
                logger.LogInformation("Feature {featureName} applied successfully", featureName);
            }

            var key = $"{containerName}-enabled-features";
            nonPersistentStorage.PutValue(key, enabledFeatures);
            logger.LogDebug("Stored enabled features under key {key}", key);

            var envKey = $"{containerName}-env";
            nonPersistentStorage.PutValue(envKey, env);
            logger.LogDebug("Stored environment variables under key {envKey}", envKey);

            logger.LogInformation("Container {containerName} startup completed", containerName);
        });
}