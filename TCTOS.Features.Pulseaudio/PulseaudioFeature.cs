using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Common;
using TCTOS.Features.Abstractions;

namespace TCTOS.Features.Pulseaudio;

public sealed class PulseaudioFeature : IFeature
{
    private const string DeviceName = "pulse";

    public Task<Result<DescribedValue<bool>>> IsApplicable(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(() =>
        {
            try
            {
                if (!featureContext.EnvironmentVariableProvider.HasVariable("XDG_RUNTIME_DIR"))
                    return Task.FromResult(new DescribedValue<bool>(false, "$XDG_RUNTIME_DIR is not set"));
                var runtimeDir = featureContext.EnvironmentVariableProvider.GetVariableValue("XDG_RUNTIME_DIR");
                var pulseFolderPath = Path.Combine(runtimeDir, "pulse");
                return Task.FromResult(!Directory.Exists(pulseFolderPath)
                    ? new DescribedValue<bool>(false, $"Directory \"{pulseFolderPath}\" does not exist")
                    : new DescribedValue<bool>(true, $"Directory \"{pulseFolderPath}\" exists"));
            }
            catch (Exception exception)
            {
                return Task.FromException<DescribedValue<bool>>(exception);
            }
        });

    public Task<Result> Apply(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            var runtimeDir = featureContext.EnvironmentVariableProvider.GetVariableValue("XDG_RUNTIME_DIR");
            var directoryPath = Path.Combine(runtimeDir, "pulse");
            (await IncusHelper.AddDeviceAsync(featureContext.IncusClient, containerName, DeviceName,
                new IncusDiskDevice
                {
                    Path = directoryPath,
                    Source = directoryPath,
                    Shift = true
                })).ThrowIfFailed();
        });

    public Task<Result> Unapply(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            (await IncusHelper.RemoveDeviceAsync(featureContext.IncusClient, containerName, DeviceName))
                .ThrowIfFailed();
        });
}