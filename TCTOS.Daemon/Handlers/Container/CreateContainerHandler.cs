using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations;

namespace TCTOS.Daemon.Handlers.Container;

public sealed class CreateContainerHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();
        
        logger.LogInformation("Handling create container message");
        
        var createMessage = (CreateContainerSocketMessage)message;
        logger.LogDebug("Create message is {message}", createMessage);
        
        var incusClient = container.Get<IIncusClient>();
        var provisioner = container.Get<IContainerProvisioner>();
        var featureProvider = container.Get<IFeatureProvider>();
        var fileSystem = container.Get<IFileSystem>();

        logger.LogDebug("Running create container operation");
        var createResult = await CreateContainerOperation.CreateContainerAsync(
            createMessage.ContainerName,
            createMessage.Color,
            createMessage.Features,
            createMessage.Description,
            createMessage.Image,
            logger,
            incusClient,
            provisioner,
            featureProvider,
            fileSystem
        );

        logger.LogDebug("Result is {result}", createResult);
        return createResult.HasFailed
            ? new SocketResponse(error: createResult.Exception!.Message)
            : new SocketResponse();
    });
}