using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Applications;

public sealed class ListExportableApplicationsHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling list exported apps message");

        var listExportableAppMessage = (ListExportableApplicationsMessage)message;

        var incusFileSystem = container.Get<IIncusFileSystem>();
        try
        {
            var preparationResult = await incusFileSystem.PrepareFileSystem(listExportableAppMessage.ContainerName);
            if (preparationResult.HasFailed)
                return new SocketResponse(error: preparationResult.Exception!.Message);
            
            List<string> exportableApps = [];
            string[] searchPaths = ["/usr/share/applications"];

            foreach (var searchPath in searchPaths)
            {
                var doesExistResult = await incusFileSystem.DoesDirectoryExistAsync(searchPath);
                if (doesExistResult.HasFailed)
                    return new SocketResponse(error: doesExistResult.Exception!.Message);
                if (!doesExistResult.GetOrThrow())
                    continue;
                var listFilesResult = await incusFileSystem.ListFilesAsync(searchPath);
                if (listFilesResult.HasFailed)
                    return new SocketResponse(error: listFilesResult.Exception!.Message);
                foreach (var filePath in listFilesResult.GetOrThrow())
                    exportableApps.AddRange(filePath);
            }

            return new SocketResponse(data: exportableApps);
        }
        finally
        {
            (await incusFileSystem.DisposeFileSystem(listExportableAppMessage.ContainerName)).ThrowIfFailed();
        }
    });
}