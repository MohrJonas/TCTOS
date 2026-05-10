using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Common;
using TCTOS.Features.Abstractions;

namespace TCTOS.Features.Wayland;

// ReSharper disable once UnusedType.Global
public sealed class WaylandFeature : IFeature
{
    private const string DeviceName = "wayland";
    
    public Task<Result<DescribedValue<bool>>> IsApplicable(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            if (!featureContext.EnvironmentVariableProvider.HasVariable("XDG_RUNTIME_DIR"))
                return new DescribedValue<bool>(false, "$XDG_RUNTIME_DIR is not set");
            if (!featureContext.EnvironmentVariableProvider.HasVariable("WAYLAND_DISPLAY"))
                return new DescribedValue<bool>(false, "$WAYLAND_DISPLAY is not set");
            var runtimeDir = featureContext.EnvironmentVariableProvider.GetVariableValue("XDG_RUNTIME_DIR");
            var socketName = featureContext.EnvironmentVariableProvider.GetVariableValue($"WAYLAND_DISPLAY");
            var socketPath = Path.Combine(runtimeDir, socketName);
            if (!File.Exists(socketPath))
                return new DescribedValue<bool>(false, $"Socket path \"{socketPath}\" does not exist");
            return new DescribedValue<bool>(true, $"Socket path \"{socketPath}\" exists");
        });

    public Task<Result> Apply(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            var runtimeDir = featureContext.EnvironmentVariableProvider.GetVariableValue("XDG_RUNTIME_DIR");
            var socketName = featureContext.EnvironmentVariableProvider.GetVariableValue($"WAYLAND_DISPLAY");
            var socketPath = Path.Combine(runtimeDir, socketName);
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
            (await IncusHelper.RemoveDeviceAsync(featureContext.IncusClient, containerName, DeviceName)).ThrowIfFailed();
        });
}