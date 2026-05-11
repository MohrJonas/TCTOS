using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Common;
using TCTOS.Features.Abstractions;
using TCTOS.Impls.Local;

namespace TCTOS.Daemon;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("Base dir: " + AppContext.BaseDirectory);
        Console.WriteLine("B.dll location: " + typeof(FeatureContext).Assembly.Location);
        
        var cancellationTokenSource = new CancellationTokenSource();
        PosixSignalRegistration.Create(PosixSignal.SIGINT, ctx =>
        {
            ctx.Cancel = true;
            cancellationTokenSource.Cancel();
        });

        var variableProvider = new LocalEnvironmentVariableProviderImpl();

        var persistentDataRootPath = variableProvider.HasVariable("TCTOS_DATA_ROOT")
            ? variableProvider.GetVariableValue("TCTOS_DATA_ROOT")
            : FixedPaths.DefaultPersistentRootPath;

        var socketPath = variableProvider.HasVariable("TCTOS_SOCKET_PATH")
            ? variableProvider.GetVariableValue("TCTOS_SOCKET_PATH")
            : FixedPaths.DefaultSocketPath;

        var logLevel = variableProvider.HasVariable("TCTOS_LOGLEVEL")
            ? Enum.Parse<LogLevel>(variableProvider.GetVariableValue("TCTOS_LOGLEVEL"), true)
            : LogLevel.Trace;

        var loggerFactory = LoggerFactory.Create(configure =>
        {
            configure.SetMinimumLevel(logLevel);
            configure.AddSimpleConsole(static configure =>
            {
                configure.IncludeScopes = true;
                configure.SingleLine = false;
                configure.ColorBehavior = LoggerColorBehavior.Enabled;
            });
        });

        var logger = loggerFactory.CreateLogger("Program");

        var fileSystem = new LocalFileSystemImpl(
            loggerFactory.CreateLogger<LocalFileSystemImpl>(),
            persistentDataRootPath
        );

        var configuration = (await fileSystem.GetConfigurationAsync()).GetOrThrow()!;

        var container = new DiContainer();
        container
            .Add<IFileSystem>(fileSystem)
            .AddLazy(ILogger () => loggerFactory.CreateLogger(string.Empty))
            .AddLazy(IIncusClient () => new LocalUnixSocketIncusClient())
            .AddLazy(IContainerProvisioner () => new AnsibleContainerProvisionerImpl())
            .AddLazy(IFeatureRunner () => new CSharpFeatureRunner())
            .AddLazy(IEnvironmentVariableProvider () => new LocalEnvironmentVariableProviderImpl())
            .AddLazy(INonPersistentStorage () => new LocalFileSystemNonPersistentStorageImpl(
                configuration.NonPersistentRoot
            ))
            .AddLazy(IUserInformationCollector () => new LocalUserInformationCollector())
            .AddLazy(ICommandRunner () => new LocalCommandRunnerImpl())
            .AddLazy(IFeatureProvider () => new DllFeatureProvider(persistentDataRootPath))
            .AddLazy(IBackgroundCommandRunner () => new SystemdBackgroundCommandRunner())
            .AddLazy(IIncusFileSystem () =>
                new SshFsIncusFileSystem(container.Get<IBackgroundCommandRunner>(), persistentDataRootPath))
            .AddLazy(IComputer () => new LocalComputerImpl(persistentDataRootPath));

        using var socketListener =
            new LocalUnixSocketListener(socketPath, loggerFactory.CreateLogger<LocalUnixSocketListener>());
        File.SetUnixFileMode(socketPath,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.GroupWrite);

        socketListener.OnMessage += message =>
        {
            logger.LogDebug("Running socket message handler");
            var task = HandleMessage(message, container, logger);
            task.Wait(cancellationTokenSource.Token);
            logger.LogTrace("Handler result is {result}", task.Result);
            return task.Result;
        };
        await socketListener.ListenAsync(cancellationTokenSource.Token);

        await ShutdownContainers(container);
    }

    private static async Task<object?> HandleMessage(SocketMessage message, DiContainer container, ILogger logger)
    {
        logger.LogTrace("Getting message handler from factory");
        var messageHandler = HandlerFactory.GetHandlerByMessageType(message.MessageType, container);
        logger.LogTrace("Message handler is {handler}", messageHandler);
        if (messageHandler == null)
        {
            logger.LogWarning("No handler for message type {type} found", message.MessageType);
            return null;
        }

        logger.LogTrace("Running message handler");
        var messageResult = await messageHandler.HandleMessage(message);
        logger.LogTrace("Message handler result is {result}", messageResult);
        return messageResult.HasFailed ? null : messageResult.GetOrThrow();
    }

    private static async Task ShutdownContainers(DiContainer container)
    {
        var logger = container.Get<ILogger>();
        var client = container.Get<IIncusClient>();
        var instancesResult = await client.GetContainersAsync();
        if (instancesResult.IsError())
        {
            logger.LogError("Error getting instances: {error}", instancesResult.Error);
            return;
        }

        var instances = instancesResult.Metadata;
        foreach (var instance in instances)
        {
            logger.LogInformation("Stopping container: {name}", instance.Name);
            var stopResult = await client.StopContainerAsync(instance.Name);
            if (stopResult.IsError())
            {
                logger.LogError("Error stopping container: {error}", stopResult.Error);
                return;
            }

            var waitResult = await client.WaitForOperationAsync(stopResult.Operation!);
            if (!waitResult.IsError()) continue;
            logger.LogError("Error waiting for container stop: {error}", waitResult.Error);
            return;
        }
        logger.LogInformation("Shutdown completed");
    }
}