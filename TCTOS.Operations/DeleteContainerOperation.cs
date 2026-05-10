using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Operations;

public static class DeleteContainerOperation
{
    public static Task<Result> DeleteContainerAsync(
        string containerName, 
        ILogger logger,
        IIncusClient incusClient,
        IFileSystem fileSystem,
        IComputer computer
    ) => RunCatchingAsync(async () =>
    {
        var existingContainerNames
            = (await incusClient.GetContainersAsync()).Metadata.Select(static i => i.Name);

        if (!existingContainerNames.Contains(containerName))
            throw new NoSuchContainerException(containerName);

        logger.LogDebug("Removing protection deletion config");
        var instance = (await incusClient.GetContainerAsync(containerName)).Metadata;
        var post = instance.ToInstancePut();
        post.Config?.Remove("security.protection.delete");

        if (instance.Status != "Stopped")
        {
            logger.LogWarning("Container is not stopped");
            throw new Exception("Container has to be stopped to be removed");   
        }
        
        var updateResponse = await incusClient.UpdateContainerAsync(containerName, post);
        updateResponse.ThrowOnError();
        (await incusClient.WaitForOperationAsync(updateResponse.Operation!)).ThrowOnError();
        
        logger.LogDebug("Deleting container");
        var deleteResponse = await incusClient.DeleteContainerAsync(containerName);
        deleteResponse.ThrowOnError();
        (await incusClient.WaitForOperationAsync(deleteResponse.Operation!)).ThrowOnError();
        
        logger.LogDebug("Removing container files");
        (await fileSystem.RemoveContainerFiles(containerName)).ThrowIfFailed();
        (await computer.RemoveContainerFiles(containerName)).ThrowIfFailed();
    });
}