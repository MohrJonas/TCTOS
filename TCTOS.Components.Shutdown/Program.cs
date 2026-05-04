using Microsoft.Extensions.Logging;
using TCTOS.Impls.Local;
using TCTOS.Operations;

namespace TCTOS.Components.Shutdown;

internal static class Program
{
    private static async Task Main()
    {
        var loggerFactory = LoggerFactory.Create(static configure =>
        {
            configure.SetMinimumLevel(LogLevel.Trace);
            configure.AddSystemdConsole(static configure =>
            {
                configure.IncludeScopes = true;
            });
        });
        var logger = loggerFactory.CreateLogger("shutdown");
        var client = new LocalUnixSocketIncusClient();
        var containers = (await client.GetContainersAsync()).Metadata;
        foreach (var container in containers)
        {
            logger.LogInformation("Stopping container {containerName}", container.Name);
            if (container.Status != "Running")
            {
                logger.LogInformation("Container not running, skipping");   
                continue;
            }
            logger.LogInformation("Stopping container");
            (await StopContainerOperation.StopContainerAsync(
                container.Name,
                logger,
                client,
                new LocalUserInformationCollector(),
                new LocalFileSystemNonPersistentStorageImpl(),
                new DllFeatureProvider(),
                new CSharpFeatureRunner(),
                new LocalFileSystemImpl(loggerFactory.CreateLogger<LocalFileSystemImpl>()),
                new LocalEnvironmentVariableProviderImpl(),
                new LocalCommandRunnerImpl(),
                new SystemdBackgroundCommandRunner()
            )).ThrowIfFailed();
            logger.LogInformation("Container stopped");
        }
    }
}