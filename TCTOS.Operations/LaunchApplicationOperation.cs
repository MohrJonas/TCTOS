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
        uint uid,
        uint gid,
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
        logger.LogInformation("Launching application {application} for {containerName}",
            string.Join(" ", commandAndArgs), containerName);

        var containerNames =
            (await incusClient.GetContainerNamesAsync()).Metadata.Select(name => name.Split("/").Last()).ToArray();

        logger.LogDebug("Available containers: {containers}", string.Join(", ", containerNames));

        if (!containerNames.Contains(containerName))
            throw new NoSuchContainerException(containerName);

        logger.LogInformation("Fetching container state for {containerName}", containerName);
        var instanceResponse = await incusClient.GetContainerAsync(containerName);
        instanceResponse.ThrowOnError();
        var instance = instanceResponse.Metadata;

        logger.LogDebug("Container {containerName} status: {status}", containerName, instance.Status);

        if (instance.Status == "Stopped")
        {
            logger.LogInformation("Container {containerName} not running, starting", containerName);
            (await StartContainerOperation.StartContainerAsync(
                containerName,
                uid,
                gid,
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
        logger.LogDebug("Env key is {envKey}", envKey);

        logger.LogInformation("Starting command in container");
        (await incusClient.RunCommand(containerName, "/sbin/simlog",
            0, 0,
            [
                "--uid", uid.ToString(),
                "--gid", gid.ToString(),
                "--", ..commandAndArgs
            ], nonPersistentStorage.PeekValue<Dictionary<string, object>>(envKey)
        )).ThrowOnError();

        logger.LogInformation("Launch in container {containerName} done", containerName);
    });
}