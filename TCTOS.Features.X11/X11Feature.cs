using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Common;
using TCTOS.Features.Abstractions;

namespace TCTOS.Features.X11;

// ReSharper disable once UnusedType.Global
public sealed class X11Feature : IFeature
{
    private const string DeviceName = "x11";

    private static string GetBackgroundJobKey(string containerName) => $"{containerName}-x11-bkgkey";

    public Task<Result<DescribedValue<bool>>> IsApplicable(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            var versionResult = await featureContext.CommandRunner.RunCommand("xwayland-satellite", ["-version"]);
            if (versionResult.HasFailed)
                return new DescribedValue<bool>(false, $"\"xwayland-satellite\" failed to start");
            var commandOutput = versionResult.GetOrThrow();
            if (commandOutput.ExitCode != 0)
                return new DescribedValue<bool>(false,
                    $"\"xwayland-satellite -version\" exited with code {commandOutput.ExitCode}");
            return new DescribedValue<bool>(true,
                $"xwayland-satellite version {commandOutput.Stdout?.Trim()} seems to work correctly");
        });

    public Task<Result> Apply(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            var displayId = Random.Shared.Next(0, short.MaxValue);
            var backgroundJobIdentifier = (await featureContext.BackgroundCommandRunner.RunCommandInBackground(
                "systemd-run",
                [
                    "--wait", 
                    "--pipe",
                    "--user", 
                    "--machine", "1000@", 
                    "xwayland-satellite", 
                    $":{displayId}", 
                    "-ac", 
                    "-nolisten", "tcp",
                    "-extension", "MIT-SHM", 
                    "+extension", "SECURITY"
                ]
            )).GetOrThrow();
            await Task.Delay(500);
            featureContext.NonPersistentStorage.PutValue(GetBackgroundJobKey(containerName), backgroundJobIdentifier);
            var socketPath = $"/tmp/.X11-unix/X{displayId}";
            (await IncusHelper.AddDeviceAsync(featureContext.IncusClient, containerName, DeviceName, new IncusDiskDevice
            {
                Path = socketPath,
                Source = socketPath,
                Shift = true
            })).ThrowIfFailed();
            featureContext.ContainerEnvironmentVariables.Add("DISPLAY", $":{displayId}");
        });

    public Task<Result> Unapply(string containerName, FeatureContext featureContext) =>
        ResultStatics.RunCatchingAsync(async () =>
        {
            (await IncusHelper.RemoveDeviceAsync(featureContext.IncusClient, containerName, DeviceName))
                .ThrowIfFailed();
        });
}