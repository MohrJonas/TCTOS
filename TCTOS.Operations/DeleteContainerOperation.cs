using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;
using TCTOS.Util;

namespace TCTOS.Operations;

public static class DeleteContainerOperation
{
    public static Task<Result> DeleteContainerAsync(
        string containerName, 
        ILogger logger,
        IIncusClient incusClient
    ) => RunCatchingAsync(async () =>
    {
        var existingContainerNames
            = (await incusClient.GetContainersAsync()).Metadata.Select(static i => i.Name);

        if (existingContainerNames.Contains(containerName))
            throw new NoSuchContainerException(containerName);

        var instance = (await incusClient.GetContainerAsync(containerName)).Metadata;
        var post = instance.ToInstancePut();
        post.Config?.Remove("security.protection.delete");

        if (instance.Status != "Stopped")
            throw new Exception("Container has to be stopped to be removed");
        
        var updateResponse = await incusClient.UpdateContainerAsync(containerName, post);
        updateResponse.ThrowOnError();
        await incusClient.WaitForOperationAsync(updateResponse.Operation!);
        
        var deleteResponse = await incusClient.DeleteContainerAsync(containerName);
        deleteResponse.ThrowOnError();
        await incusClient.WaitForOperationAsync(deleteResponse.Operation!);
    });
}