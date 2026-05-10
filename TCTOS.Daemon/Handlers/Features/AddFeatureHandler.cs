using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Features;

public sealed class AddFeatureHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling add feature message");

        var addFeatureMessage = (AddFeatureSocketMessage)message;

        var featureProvider = container.Get<IFeatureProvider>();
        var availableFeaturesResult = await featureProvider.GetAvailableFeaturesAsync();
        if (availableFeaturesResult.HasFailed)
            return new SocketResponse(error: availableFeaturesResult.Exception!.Message);
        var availableFeatureNames = availableFeaturesResult.GetOrThrow().Select(f => f.Name);

        if (!availableFeatureNames.Contains(addFeatureMessage.FeatureName))
            return new SocketResponse(error: $"Feature \"{addFeatureMessage.FeatureName}\" does not exist");
        
        var fileSystem = container.Get<IFileSystem>();

        var containerConfigurationResult =
            await fileSystem.GetContainerConfigurationAsync(addFeatureMessage.ContainerName);
        if (containerConfigurationResult.HasFailed)
            return new SocketResponse(error: containerConfigurationResult.Exception!.Message);

        var containerConfiguration = containerConfigurationResult.GetOrThrow();
        if (containerConfiguration == null)
            return new SocketResponse(
                error: $"Configuration for container {addFeatureMessage.ContainerName} not found");

        containerConfiguration.FeatureNames = [..containerConfiguration.FeatureNames, addFeatureMessage.FeatureName];

        var setConfigurationResult =
            await fileSystem.SetContainerConfigurationAsync(addFeatureMessage.ContainerName, containerConfiguration);
        
        return setConfigurationResult.HasFailed
            ? new SocketResponse(error: setConfigurationResult.Exception!.Message)
            : new SocketResponse();
    });
}