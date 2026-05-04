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
            await client.UpdateContainerAsync(containerName, instancePut);
        });

    public static Task<Result> RemoveDeviceAsync(IIncusClient client, string containerName, string deviceName) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            var instancePut = (await client.GetContainerAsync(containerName)).Metadata.ToInstancePut();
            instancePut.Devices?.Remove(deviceName);
            await client.UpdateContainerAsync(containerName, instancePut);
        });
}