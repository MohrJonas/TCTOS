using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Container;

public sealed class ListContainersHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();
        
        logger.LogInformation("Handling launch message");
            
        var listMessage = (ListContainersSocketMessage)message;
        logger.LogDebug("List message is {message}", listMessage);
        
        var incusClient = container.Get<IIncusClient>();
        var fileSystem = container.Get<IFileSystem>();

        var containersResult = await incusClient.GetContainersAsync();
        if (containersResult.IsError())
        {
            logger.LogError("Getting containers failed: {err}", containersResult.Error);
            return new SocketResponse(error: containersResult.Error);
        }
            
        var containers = containersResult.Metadata;
        logger.LogDebug("Containers are {containers}", string.Join(", ", containers.Select(c => c.Name)));
        
        List<Abstractions.Data.Container> containerObjects = [];
        foreach (var instance in containers)
        {
            logger.LogTrace("Getting configuration for container {containerName}", instance.Name);
            var configuration = await fileSystem.GetContainerConfigurationAsync(instance.Name);
            if (configuration.HasFailed)
                return new SocketResponse(error: configuration.Exception!.Message);
            if (configuration.GetOrThrow() == null)
                return new SocketResponse(error: $"Configuration for container {instance.Name} not found");
            containerObjects.Add(new Abstractions.Data.Container(
                instance.Name,
                instance.Description,
                instance.Status,
                configuration.GetOrThrow()!.FeatureNames
            ));
        }

        var response = new SocketResponse(data: containerObjects.ToArray());
        logger.LogDebug("Response is {response}", response);

        return response;
    });
}