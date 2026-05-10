using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Common;
using TCTOS.Features.Abstractions;

namespace TCTOS.Features.Pipewire;

// ReSharper disable once UnusedType.Global
public sealed class PipewireFeature : IFeature
{
    private const string DeviceName = "pipewire";

    public Task<Result<DescribedValue<bool>>> IsApplicable(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(() =>
        {
            try
            {
                if (!featureContext.EnvironmentVariableProvider.HasVariable("XDG_RUNTIME_DIR"))
                    return Task.FromResult(new DescribedValue<bool>(false, "$XDG_RUNTIME_DIR is not set"));
                var runtimeDir = featureContext.EnvironmentVariableProvider.GetVariableValue("XDG_RUNTIME_DIR");
                var pipewireSocketPath = Path.Combine(runtimeDir, "pipewire-0");
                return Task.FromResult(!File.Exists(pipewireSocketPath) 
                    ? new DescribedValue<bool>(false, $"Socket path \"{pipewireSocketPath}\" does not exist") 
                    : new DescribedValue<bool>(true, $"Socket path \"{pipewireSocketPath}\" exists"));
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
            var pipewireSocketPath = Path.Combine(runtimeDir, "pipewire-0");
            (await IncusHelper.AddDeviceAsync(featureContext.IncusClient, containerName, DeviceName, new IncusDiskDevice
            {
                Source = pipewireSocketPath,
                Path = pipewireSocketPath,
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