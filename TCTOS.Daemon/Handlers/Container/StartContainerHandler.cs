using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations;

namespace TCTOS.Daemon.Handlers.Container;

public sealed class StartContainerHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();
        
        logger.LogInformation("Handling start container message");
        
        var startMessage = (StartContainerSocketMessage)message;
        logger.LogDebug("Start message is {message}", startMessage);
        
        var incusClient = container.Get<IIncusClient>();
        var userInformationCollector = container.Get<IUserInformationCollector>();
        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();
        var variableProvider = container.Get<IEnvironmentVariableProvider>();
        var runner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();

        logger.LogDebug("Running start container operation");
        var startResult = await StartContainerOperation.StartContainerAsync(
            startMessage.ContainerName,
            startMessage.Uid,
            startMessage.Gid,
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

        logger.LogDebug("Result is {result}", startResult);
        return startResult.HasFailed
            ? new SocketResponse(error: startResult.Exception!.Message)
            : new SocketResponse();
    });
}