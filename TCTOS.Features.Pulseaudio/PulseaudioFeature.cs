using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Common;
using TCTOS.Features.Abstractions;

namespace TCTOS.Features.Pulseaudio;

// ReSharper disable once UnusedType.Global
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
                var pulseSocketPath = Path.Combine(runtimeDir, "pulse", "native");
                return Task.FromResult(!Directory.Exists(pulseSocketPath)
                    ? new DescribedValue<bool>(false, $"Socket path \"{pulseSocketPath}\" does not exist")
                    : new DescribedValue<bool>(true, $"Socket path \"{pulseSocketPath}\" exists"));
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
            var socketPath = Path.Combine(runtimeDir, "pulse", "native");
            (await IncusHelper.AddDeviceAsync(featureContext.IncusClient, containerName, DeviceName,
                new IncusDiskDevice
                {
                    Path = socketPath,
                    Source = socketPath,
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