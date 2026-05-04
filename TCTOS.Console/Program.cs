using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
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
        ILoggerFactory loggerFactory = null!;
        var container = new DiContainer();
        container
            .AddLazy(ILogger () => loggerFactory.CreateLogger(string.Empty))
            .AddLazy(IIncusClient () => new LocalUnixSocketIncusClient())
            .AddLazy(IContainerProvisioner () => new AnsibleContainerProvisionerImpl())
            .AddLazy(IFeatureRunner () => new CSharpFeatureRunner())
            .AddLazy(IEnvironmentVariableProvider () => new LocalEnvironmentVariableProviderImpl())
            .AddLazy(IFileSystem () => new LocalFileSystemImpl(loggerFactory.CreateLogger<LocalFileSystemImpl>()))
            .AddLazy(INonPersistentStorage () => new LocalFileSystemNonPersistentStorageImpl())
            .AddLazy(IUserInformationCollector () => new LocalUserInformationCollector())
            .AddLazy(ICommandRunner () => new LocalCommandRunnerImpl())
            .AddLazy(IFeatureProvider () => new DllFeatureProvider())
            .AddLazy(IBackgroundCommandRunner () => new SystemdBackgroundCommandRunner())
            .AddLazy(IIncusFileSystem () =>
                new SshFsIncusFileSystem(container.Get<IBackgroundCommandRunner>()));

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