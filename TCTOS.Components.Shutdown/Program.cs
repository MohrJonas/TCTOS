using Microsoft.Extensions.Logging;
using TCTOS.Common;
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
            configure.AddSystemdConsole(static configure => { configure.IncludeScopes = true; });
        });

        var logger = loggerFactory.CreateLogger("shutdown");
        var client = new LocalUnixSocketIncusClient();
        
        var persistentRootPath =
            Environment.GetEnvironmentVariable("TCTOS_ROOT") ?? FixedPaths.DefaultPersistentRootPath;
        var fileSystem = new LocalFileSystemImpl(
            loggerFactory.CreateLogger<LocalFileSystemImpl>(),
            persistentRootPath
        );
        
        var configuration = (await fileSystem.GetConfigurationAsync()).GetOrThrow()!;
        
        var containers = (await client.GetContainersAsync()).Metadata;
        foreach (var container in containers)
        {
            logger.LogInformation("Stopping container {containerName}", container.Name);
            (await StopContainerOperation.StopContainerAsync(
                container.Name,
                logger,
                client,
                new LocalUserInformationCollector(),
                new LocalFileSystemNonPersistentStorageImpl(configuration.NonPersistentRoot),
                new DllFeatureProvider(persistentRootPath),
                new CSharpFeatureRunner(),
                fileSystem,
                new LocalEnvironmentVariableProviderImpl(),
                new LocalCommandRunnerImpl(),
                new SystemdBackgroundCommandRunner()
            )).ThrowIfFailed();
            logger.LogInformation("Container stopped");
        }
    }
}