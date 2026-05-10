using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations;

namespace TCTOS.Daemon.Handlers.Container;

public sealed class DeleteContainersHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();
        
        logger.LogInformation("Handling delete container message");

        var deleteMessage = (DeleteContainerSocketMessage)message;
        logger.LogDebug("Delete message is {message}", deleteMessage);
        
        var incusClient = container.Get<IIncusClient>();
        var fileSystem = container.Get<IFileSystem>();
        var computer = container.Get<IComputer>();
        
        logger.LogDebug("Running delete container operation");
        var deleteResult = await DeleteContainerOperation.DeleteContainerAsync(
            deleteMessage.ContainerName,
            logger, incusClient, fileSystem, computer
        );

        logger.LogDebug("Result is {result}", deleteResult);
        return deleteResult.HasFailed
            ? new SocketResponse(error: deleteResult.Exception!.Message)
            : new SocketResponse();
    });
}