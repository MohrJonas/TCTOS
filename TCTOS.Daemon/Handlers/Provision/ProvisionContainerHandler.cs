using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Common;

namespace TCTOS.Daemon.Handlers.Provision;

public sealed class ProvisionContainerHandler(DiContainer container) : IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message) => RunCatchingAsync(async () =>
    {
        var logger = container.Get<ILogger>();

        logger.LogInformation("Handling provision message");

        var provisionMessage = (ProvisionSocketMessage)message;
        logger.LogDebug("List message is {message}", provisionMessage);

        var provisioner = container.Get<IContainerProvisioner>();
        var fileSystem = container.Get<IFileSystem>();

        logger.LogDebug("Getting provision file content");
        var provisionFileContentResult =
            await fileSystem.GetProvisioningFileContentAsync(provisionMessage.ContainerName);
        if (provisionFileContentResult.HasFailed)
        {
            logger.LogError("Getting provision file content has failed");
            return new SocketResponse(error: provisionFileContentResult.Exception!.Message);
        }

        var provisionFileContent = provisionFileContentResult.GetOrThrow();
        if (provisionFileContent == null)
        {
            logger.LogError("Provision file content is null");
            return new SocketResponse(error: "Provision file content is null");
        }

        Dictionary<string, string> variables = new()
        {
            { "TCTOS_GID", provisionMessage.Gid.ToString() },
            { "TCTOS_UID", provisionMessage.Uid.ToString() },
            { "ansible_python_interpreter", "auto_silent" }
        };

        logger.LogDebug("Variables are: {variables}",
            string.Join(", ", variables.Select(v => $"{v.Key} => {v.Value}")));

        var provisionResult = await provisioner.ProvisionContainer(
            provisionMessage.ContainerName,
            provisionFileContent,
            variables
        );
        
        logger.LogDebug("Result is {result}", provisionResult);
        return provisionResult.HasFailed
            ? new SocketResponse(error: provisionResult.Exception!.Message)
            : new SocketResponse();
    });
}