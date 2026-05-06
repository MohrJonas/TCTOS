using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Console.Commands;
using TCTOS.Impls.Local;

namespace TCTOS.Console;

internal static class Program
{
    private static ILoggerFactory CreateDefaultLoggerFactory(LogLevel logLevel)
    {
        return LoggerFactory.Create(configure =>
        {
            configure.SetMinimumLevel(logLevel);
            configure.AddSimpleConsole(static configure =>
            {
                configure.SingleLine = false;
                configure.IncludeScopes = true;
            });
        });
    }

    private static async Task<int> Main(string[] args)
    {
        var loggerFactory = CreateDefaultLoggerFactory(LogLevel.Warning);

        var persistentRootPath =
            Environment.GetEnvironmentVariable("TCTOS_ROOT") ?? FixedPaths.DefaultPersistentRootPath;
        var fileSystem = new LocalFileSystemImpl(
            loggerFactory.CreateLogger<LocalFileSystemImpl>(),
            persistentRootPath
        );

        var configuration = (await fileSystem.GetConfigurationAsync()).GetOrThrow()!;

        var container = new DiContainer();
        container
            .Add((IFileSystem)fileSystem)
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
            .AddLazy(IFeatureProvider () => new DllFeatureProvider(persistentRootPath))
            .AddLazy(IBackgroundCommandRunner () => new SystemdBackgroundCommandRunner())
            .AddLazy(IIncusFileSystem () =>
                new SshFsIncusFileSystem(container.Get<IBackgroundCommandRunner>()))
            .AddLazy(IComputer () => new LocalComputerImpl());

        var command = new BaseCommand(container);

        var parseResult = command.Parse(args);
        foreach (var error in parseResult.Errors)
            await System.Console.Error.WriteLineAsync(error.Message);
        if (parseResult.Errors.Count != 0)
            Environment.Exit(1);

        loggerFactory = CreateDefaultLoggerFactory(
            parseResult.RootCommandResult.GetRequiredValue(SharedOptions.VerboseOption)
                ? LogLevel.Trace
                : LogLevel.Error
        );

        return await parseResult.InvokeAsync();
    }
}