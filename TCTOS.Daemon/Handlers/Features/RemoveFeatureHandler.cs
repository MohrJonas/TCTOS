using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Features;

public sealed class RemoveFeatureHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling remove feature message");

        var removeFeatureMessage = (RemoveFeatureSocketMessage)message;
        
        var fileSystem = container.Get<IFileSystem>();

        var containerConfigurationResult =
            await fileSystem.GetContainerConfigurationAsync(removeFeatureMessage.ContainerName);
        if (containerConfigurationResult.HasFailed)
            return new SocketResponse(error: containerConfigurationResult.Exception!.Message);

        var containerConfiguration = containerConfigurationResult.GetOrThrow();
        if (containerConfiguration == null)
            return new SocketResponse(
                error: $"Configuration for container {removeFeatureMessage.ContainerName} not found");

        if (!containerConfiguration.FeatureNames.Contains(removeFeatureMessage.FeatureName))
            return new SocketResponse(error: $"Feature \"{removeFeatureMessage.FeatureName}\" is not enabled");
        
        containerConfiguration.FeatureNames = containerConfiguration.FeatureNames
            .Where(n => n != removeFeatureMessage.FeatureName).ToArray();
        
        var setConfigurationResult =
            await fileSystem.SetContainerConfigurationAsync(removeFeatureMessage.ContainerName, containerConfiguration);
        
        return setConfigurationResult.HasFailed
            ? new SocketResponse(error: setConfigurationResult.Exception!.Message)
            : new SocketResponse();
    });
}