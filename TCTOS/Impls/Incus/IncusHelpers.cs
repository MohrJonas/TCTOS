using TCTOS.Abstractions;
using TCTOS.Impls.Incus.DTOs;
using TCTOS.Util;

namespace TCTOS.Impls.Incus;

public static class IncusHelpers
{
    public static Task<Result> UpdateContainer(IIncusClient client, string containerName, Action<Instance> action)
    {
        return RunCatchingAsync(async () =>
        {
            var response = await client.GetContainerAsync(containerName);
            response.ThrowOnError();
            var instance = response.Metadata;
            action(instance);
            var updateResponse = await client.UpdateContainerAsync(containerName, instance.ToInstancePut());
            (await client.WaitForOperationAsync(updateResponse.Operation!)).ThrowOnError();
        });
    }
}