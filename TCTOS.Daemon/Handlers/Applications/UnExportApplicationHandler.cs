using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations;

namespace TCTOS.Daemon.Handlers.Applications;

public sealed class UnExportApplicationHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling list exported apps message");

        var unExportAppMessage = (UnExportApplicationSocketMessage)message;

        var computer = container.Get<IComputer>();
        var incusFileSystem = container.Get<IIncusFileSystem>();

        var unExportResult = await ExportApplicationOperation.UnexportApplicationAsync(unExportAppMessage.ContainerName,
            unExportAppMessage.ApplicationPath, computer);

        return unExportResult.HasFailed
            ? new SocketResponse(error: unExportResult.Exception!.Message)
            : new SocketResponse();
    });
}