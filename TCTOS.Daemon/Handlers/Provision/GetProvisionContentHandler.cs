using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Daemon.Handlers.Provision;

public sealed class GetProvisionContentHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling get provision content message");

        var getProvisionMessage = (GetProvisionContentSocketMessage)message;
        logger.LogDebug("GetProvision message is {message}", getProvisionMessage);
        
        var incusClient = container.Get<IIncusClient>();
        
        var containerNames =
            (await incusClient.GetContainerNamesAsync()).Metadata.Select(n => n.Split("/").Last()).ToArray();

        logger.LogDebug("Available containers: {containers}", string.Join(", ", containerNames));

        if (!containerNames.Contains(getProvisionMessage.ContainerName))
            return new SocketResponse(error: new NoSuchContainerException(getProvisionMessage.ContainerName).Message);

        var fileSystem = container.Get<IFileSystem>();

        var contentResult = await fileSystem.GetProvisioningFileContentAsync(getProvisionMessage.ContainerName);
        if (contentResult.HasFailed)
            return new SocketResponse(error: contentResult.Exception!.Message);

        var content = contentResult.GetOrThrow();
        return content == null
            ? new SocketResponse(error: $"Provision file for container {getProvisionMessage.ContainerName} not found")
            : new SocketResponse(data: content);
    });
}