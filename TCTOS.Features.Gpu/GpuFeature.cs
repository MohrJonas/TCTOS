using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Common;
using TCTOS.Features.Abstractions;

namespace TCTOS.Features.Gpu;

public sealed class GpuFeature : IFeature
{
    private const string DeviceName = "gpu";

    public Task<Result<DescribedValue<bool>>> IsApplicable(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(() =>
        {
            try
            {
                return Task.FromResult(!Directory.Exists("/dev/dri")
                    ? new DescribedValue<bool>(false, "Directory /dev/dri does not exist")
                    : new DescribedValue<bool>(true, "Directory /dev/dri exists"));
            }
            catch (Exception exception)
            {
                return Task.FromException<DescribedValue<bool>>(exception);
            }
        });

    public Task<Result> Apply(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            (await IncusHelper.AddDeviceAsync(featureContext.IncusClient, containerName, DeviceName,
                new IncusDiskDevice
                {
                    Path = "/dev/dri",
                    Source = "/dev/dri"
                })).ThrowIfFailed();
        });

    public Task<Result> Unapply(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            (await IncusHelper.RemoveDeviceAsync(featureContext.IncusClient, containerName, DeviceName))
                .ThrowIfFailed();
        });
}