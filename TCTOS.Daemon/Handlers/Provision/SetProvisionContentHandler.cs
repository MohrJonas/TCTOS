using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Daemon.Handlers.Provision;

public sealed class SetProvisionContentHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling set provision content message");

        var setProvisionMessage = (SetProvisionContentSocketMessage)message;
        logger.LogDebug("SetProvision message is {message}", setProvisionMessage);
        
        var incusClient = container.Get<IIncusClient>();
        
        var containerNames =
            (await incusClient.GetContainerNamesAsync()).Metadata.Select(n => n.Split("/").Last()).ToArray();

        logger.LogDebug("Available containers: {containers}", string.Join(", ", containerNames));

        if (!containerNames.Contains(setProvisionMessage.ContainerName))
            return new SocketResponse(error: new NoSuchContainerException(setProvisionMessage.ContainerName).Message);

        var fileSystem = container.Get<IFileSystem>();

        var contentResult =
            await fileSystem.SetProvisioningFileContentAsync(setProvisionMessage.ContainerName,
                setProvisionMessage.Content);
        
        return contentResult.HasFailed
            ? new SocketResponse(error: contentResult.Exception!.Message)
            : new SocketResponse();
    });
}