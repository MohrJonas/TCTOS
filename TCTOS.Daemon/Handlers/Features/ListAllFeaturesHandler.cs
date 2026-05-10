using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Features;

public sealed class ListAllFeaturesHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling listAllFeatures message");

        var featureProvider = container.Get<IFeatureProvider>();

        var featuresResult = await featureProvider.GetAvailableFeaturesAsync();
        return featuresResult.HasFailed
            ? new SocketResponse(error: featuresResult.Exception!.Message)
            : new SocketResponse(data: featuresResult.GetOrThrow().ToArray());
    });
}