using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations;

namespace TCTOS.Daemon.Handlers.Container;

public sealed class StopContainerHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();
        
        logger.LogInformation("Handling start container message");
        
        var stopMessage = (StopContainerSocketMessage)message;
        logger.LogDebug("Stop message is {message}", stopMessage);

        var incusClient = container.Get<IIncusClient>();
        var userInformationCollector = container.Get<IUserInformationCollector>();
        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();
        var variableProvider = container.Get<IEnvironmentVariableProvider>();
        var runner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();

        logger.LogDebug("Running stop container operation");
        var stopResult = await StopContainerOperation.StopContainerAsync(
            stopMessage.ContainerName,
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
        );

        logger.LogDebug("Result is {result}", stopResult);
        return stopResult.HasFailed
            ? new SocketResponse(error: stopResult.Exception!.Message)
            : new SocketResponse();
    });
}