using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Operations;

public static class LaunchApplicationOperation
{
    public static Task<Result> LaunchApplicationAsync(
        ILogger logger,
        string containerName,
        string[] commandAndArgs,
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
        var containerNames =
            (await incusClient.GetContainerNamesAsync()).Metadata.Select(name => name.Split("/").Last());
        if (!containerNames.Contains(containerName))
            throw new NoSuchContainerException(containerName);

        var instanceResponse = (await incusClient.GetContainerAsync(containerName));
        instanceResponse.ThrowOnError();
        var instance = instanceResponse.Metadata;

        if (instance.Status == "Stopped")
        {
            (await StartContainerOperation.StartContainerAsync(
                containerName, 
                logger, 
                incusClient, 
                userInformationCollector, 
                nonPersistentStorage, 
                featureProvider,
                featureRunner, 
                fileSystem, 
                environmentVariableProvider, 
                commandRunner, 
                backgroundCommandRunner
            )).ThrowIfFailed();
        }
        
        var envKey = $"{containerName}-env";
        
        (await incusClient.RunCommand(containerName, "/sbin/simlog",
            [
                "--uid", userInformationCollector.GetUid().ToString(),
                "--gid", userInformationCollector.GetGid().ToString(),
                "--", ..commandAndArgs
            ], nonPersistentStorage.PeekValue<Dictionary<string, object>>(envKey)
        )).ThrowOnError();
    });
}