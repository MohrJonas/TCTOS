using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;
using TCTOS.Operations;

namespace TCTOS.Daemon.Handlers.Applications;

public sealed class ExportApplicationHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling list exported apps message");

        var exportAppMessage = (ExportApplicationSocketMessage)message;

        var computer = container.Get<IComputer>();
        var incusFileSystem = container.Get<IIncusFileSystem>();
        var fileSystem = container.Get<IFileSystem>();
        
        var exportResult = await ExportApplicationOperation.ExportApplicationAsync(
            exportAppMessage.ContainerName,
            exportAppMessage.ApplicationPath,
            computer, incusFileSystem, fileSystem
        );

        return exportResult.HasFailed
            ? new SocketResponse(error: exportResult.Exception!.Message)
            : new SocketResponse();
    });
}