using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Operations;

public static class StopContainerOperation
{
    public static Task<Result> StopContainerAsync(
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
    ) => RunCatchingAsync(async () =>
    {
        var containerNames = (await incusClient.GetContainerNamesAsync()).Metadata.Select(name => name.Split("/").Last());
        if (!containerNames.Contains(containerName))
            throw new NoSuchContainerException(containerName);

        var instanceResponse = await incusClient.GetContainerAsync(containerName);
        instanceResponse.ThrowOnError();
        var instance = instanceResponse.Metadata;

        if (instance.Status == "Running")
        {
            var stopResponse = await incusClient.StopContainerAsync(containerName);
            stopResponse.ThrowOnError();
            (await incusClient.WaitForOperationAsync(stopResponse.Operation!)).ThrowOnError();   
        }

        var key = $"{containerName}-enabled-features";
        var enabledFeatures = nonPersistentStorage.GetValue<string[]>(key);

        foreach (var featureName in enabledFeatures)
        {
            var featureText = (await featureProvider.GetFeatureScriptTextAsync(featureName)).GetOrThrow();
            await featureRunner.UnapplyFeature(
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
        }
    });
}