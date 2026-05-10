using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations;

namespace TCTOS.Daemon.Handlers;

public sealed class LaunchHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();
        
        logger.LogInformation("Handling launch message");
        
        var launchMessage = (LaunchSocketMessage)message;
        logger.LogDebug("Launch message is {message}", launchMessage);
        
        var incusClient = container.Get<IIncusClient>();
        var userInformationCollector = container.Get<IUserInformationCollector>();
        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();
        var variableProvider = container.Get<IEnvironmentVariableProvider>();
        var runner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();

        logger.LogDebug("Running launch container operation");
        var launchResult = await LaunchApplicationOperation.LaunchApplicationAsync(
            logger,
            launchMessage.ContainerName,
            [launchMessage.Command, ..launchMessage.Args ?? []],
            launchMessage.Uid,
            launchMessage.Gid,
            incusClient,
            userInformationCollector,
            nonPersistentStorage,
            featureProvider,
            featureRunner,
            fileSystem,
            variableProvider,
            runner,
            backgroundRunner
        );

        logger.LogDebug("Result is {result}", launchResult);
        return launchResult.HasFailed
            ? new SocketResponse(error: launchResult.Exception!.Message)
            : new SocketResponse();
    });
}