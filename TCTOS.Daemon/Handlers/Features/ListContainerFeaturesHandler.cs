using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Features;

public sealed class ListContainerFeaturesHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling list container features message");

        var listFeaturesMessage = (ListContainerFeaturesSocketMessage)message;

        var fileSystem = container.Get<IFileSystem>();

        var containerConfigurationResult = await fileSystem.GetContainerConfigurationAsync(listFeaturesMessage.ContainerName);
        if (containerConfigurationResult.HasFailed)
            return new SocketResponse(error: containerConfigurationResult.Exception!.Message);

        var containerConfiguration = containerConfigurationResult.GetOrThrow();
        if(containerConfiguration == null)
            return new SocketResponse(error: $"Configuration for container {listFeaturesMessage.ContainerName} not found");

        return new SocketResponse(data: containerConfiguration.FeatureNames);
    });
}