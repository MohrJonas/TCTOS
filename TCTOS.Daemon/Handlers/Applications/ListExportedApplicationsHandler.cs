using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Applications;

public sealed class ListExportedApplicationsHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling list exported apps message");

        var listAppsMessage = (ListExportedApplicationsSocketMessage)message;

        var computer = container.Get<IComputer>();

        var listResult = await computer.ListDesktopFilesAsync(listAppsMessage.ContainerName);
        
        return listResult.HasFailed
            ? new SocketResponse(error: listResult.Exception!.Message)
            : new SocketResponse(data: listResult.GetOrThrow());
    });
}