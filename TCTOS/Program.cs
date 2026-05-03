using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Commands;
using TCTOS.Impls;
using TCTOS.Impls.Incus;
using TCTOS.IOC;

namespace TCTOS;

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
            .AddLazy(IIncusClient () => new LocalUnixSocketIncusClient())
            .AddLazy(IContainerProvisioner () => new AnsibleContainerProvisionerImpl())
            .AddLazy(IFeatureRunner () => new JavascriptFeatureRunner())
            .AddLazy(IEnvironmentVariableProvider () => new LocalEnvironmentVariableProviderImpl())
            .AddLazy(IFileSystem () => new LocalFileSystemImpl(loggerFactory.CreateLogger<LocalFileSystemImpl>()))
            .AddLazy(INonPersistentStorage () => new LocalFileSystemNonPersistentStorageImpl())
            .AddLazy(IUserInformationCollector () => new LocalUserInformationCollector())
            .AddLazy(ICommandRunner () => new LocalCommandRunnerImpl())
            .AddLazy(IFeatureProvider () => new LocalFileSystemFeatureProvider())
            .AddLazy(IBackgroundCommandRunner () => new SystemdBackgroundCommandRunner())
            .AddLazy(IIncusFileSystem () =>
                new SshFsIncusFileSystem(container.Get<IBackgroundCommandRunner>()));

        var command = new BaseCommand(container);

        var parseResult = command.Parse(args);
        foreach (var error in parseResult.Errors)
            await Console.Error.WriteLineAsync(error.Message);
        if (parseResult.Errors.Count != 0)
            Environment.Exit(1);

        loggerFactory = CreateDefaultLoggerFactory(
            parseResult.RootCommandResult.GetRequiredValue(SharedOptions.VerboseOption)
                ? LogLevel.Trace
                : LogLevel.Error
        );

        return await parseResult.InvokeAsync();

        /*var client = new IncusClient(
            DefaultLoggerFactory(LogLevel.Trace).CreateLogger<IncusClient>()
        );
        var instance = (await client.GetInstanceByNameAsync("ubuntu")).GetOrThrow();
        var put = new InstancePut
        {
            Devices = instance.Devices
        };
        put.Devices.Add("x11", new IncusDiskDevice()
        {
            Path = "/host/.X11-unix",
            Source = "/tmp/.X11-unix",
            Pool = "asd",
            Shift = true
        });
        (await client.UpdateContainer("ubuntu", put)).ThrowIfFailed();
        return;
        var verboseOption = new Option<bool>("--verbose", "-v")
        {
            Description = "Enable verbose output",
            DefaultValueFactory = _ => false
        };
        var rootCommand = new RootCommand("TheCOntainerOS");

        var containerNameArgument = new Argument<string>("container_name")
        {
            Description = "The container to provision"
        };

        var provisionCommand = new Command("provision")
        {
            Description = "Provision the selected container",
        };
        provisionCommand.Arguments.Add(containerNameArgument);
        provisionCommand.SetAction(res =>
        {
            var provisioner = new AnsibleContainerProvisionerImpl();
            provisioner.ProvisionContainer(
                res.GetRequiredValue(containerNameArgument),
                File.ReadAllText("/home/jonas/Development/TLCOS/playbook.yml"),
                new Dictionary<string, string>
                {
                    { "TCTOS_GID", CInterop.getgid().ToString() },
                    { "TCTOS_UID", CInterop.getuid().ToString() },
                    { "ansible_python_interpreter", "auto_silent" }
                }
            ).WaitAndGet().ThrowIfFailed();
        });

        var deleteCommand = new Command("delete")
        {
            Description = "Delete the selected container"
        };
        deleteCommand.Arguments.Add(containerNameArgument);
        deleteCommand.SetAction(res =>
        {
            var loggerFactory = DefaultLoggerFactory(
                res.RootCommandResult.GetRequiredValue(verboseOption)
                    ? LogLevel.Trace
                    : LogLevel.Information
            );
            var client = new IncusClient(loggerFactory.CreateLogger<IncusClient>());
            client.DeleteContainerByNameAsync(res.GetRequiredValue(containerNameArgument))
                .WaitAndGet()
                .ThrowIfFailed();
        });

        var startCommand = new Command("start")
        {
            Description = "Start the selected container"
        };
        startCommand.Arguments.Add(containerNameArgument);
        startCommand.SetAction(res =>
        {
            var loggerFactory = DefaultLoggerFactory(
                res.RootCommandResult.GetRequiredValue(verboseOption)
                    ? LogLevel.Trace
                    : LogLevel.Information
            );
            var client = new IncusClient(loggerFactory.CreateLogger<IncusClient>());
            client.StartContainerByNameAsync(res.GetRequiredValue(containerNameArgument))
                .WaitAndGet()
                .ThrowIfFailed();
        });

        var stopCommand = new Command("stop")
        {
            Description = "Stop the selected container"
        };
        stopCommand.Arguments.Add(containerNameArgument);
        stopCommand.SetAction(res =>
        {
            var loggerFactory = DefaultLoggerFactory(
                res.RootCommandResult.GetRequiredValue(verboseOption)
                    ? LogLevel.Trace
                    : LogLevel.Information
            );
            var client = new IncusClient(loggerFactory.CreateLogger<IncusClient>());
            client.StopContainerByNameAsync(res.GetRequiredValue(containerNameArgument))
                .WaitAndGet()
                .ThrowIfFailed();
        });

        var lsCommand = new Command("ls")
        {
            Description = "List available container"
        };
        lsCommand.SetAction(res =>
        {
            var loggerFactory = DefaultLoggerFactory(
                res.RootCommandResult.GetRequiredValue(verboseOption)
                    ? LogLevel.Trace
                    : LogLevel.Information
            );
            var client = new IncusClient(loggerFactory.CreateLogger<IncusClient>());
            var containerNames = client.ListAvailableContainerNamesAsync()
                .WaitAndGet()
                .GetOrThrow();
            foreach (var containerName in containerNames)
                Console.WriteLine($"- {containerName}");
        });

        var editCommand = new Command("edit")
        {
            Description = "Edit the config of the container"
        };
        editCommand.SetAction(_ =>
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = Environment.GetEnvironmentVariable("EDITOR"),
                Arguments = "/home/jonas/Development/TLCOS/playbook.yml"
            };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            Console.WriteLine(process?.ExitCode.ToString() ?? "??????");
        });*/
    }
}