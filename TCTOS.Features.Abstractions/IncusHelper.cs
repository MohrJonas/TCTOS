using TCTOS.Abstractions;
using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Common;

namespace TCTOS.Features.Abstractions;

public static class IncusHelper
{
    public static Task<Result> AddDeviceAsync(IIncusClient client, string containerName, string deviceName, IIncusDevice device) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            var instancePut = (await client.GetContainerAsync(containerName)).Metadata.ToInstancePut();
            instancePut.Devices?.Remove(deviceName);
            instancePut.Devices?.Add(deviceName, device);
            var response = await client.UpdateContainerAsync(containerName, instancePut);
            await client.WaitForOperationAsync(response.Operation!);
        });

    public static Task<Result> RemoveDeviceAsync(IIncusClient client, string containerName, string deviceName) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            var instancePut = (await client.GetContainerAsync(containerName)).Metadata.ToInstancePut();
            instancePut.Devices?.Remove(deviceName);
            var response = await client.UpdateContainerAsync(containerName, instancePut);
            await client.WaitForOperationAsync(response.Operation!);
        });
}