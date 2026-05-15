using System.CommandLine;
using System.Diagnostics;
using TCTOS.Impls.Local;

namespace TCTOS.Console.Commands.Container;

public static class ContainerShellCommandOptions
{
    public static readonly Option<uint> UidOption = new("--uid")
    {
        Description = "Uid of the user to spawn the shell for",
        DefaultValueFactory = _ => (uint)CInterop.getuid()
    };

    public static readonly Option<uint> GidOption = new("--gid")
    {
        Description = "Gid of the user to spawn the shell for",
        DefaultValueFactory = _ => (uint)CInterop.getgid()
    };

    public static readonly Option<string> ShellOption = new("--shell")
    {
        Description = "The shell to use",
        DefaultValueFactory = _ => "/bin/bash"
    };
}

public sealed class ContainerShellCommand() : CommandBase("shell", "Start an interactive terminal session",
    arguments: [SharedArguments.ContainerNameArgument],
    options:
    [
        ContainerShellCommandOptions.UidOption, ContainerShellCommandOptions.GidOption,
        ContainerShellCommandOptions.ShellOption
    ])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var shell = parseResult.GetRequiredValue(ContainerShellCommandOptions.ShellOption);
        var uid = parseResult.GetRequiredValue(ContainerShellCommandOptions.UidOption);
        var gid = parseResult.GetRequiredValue(ContainerShellCommandOptions.GidOption);

        var processInfo = new ProcessStartInfo
        {
            FileName = "incus",
            Arguments = string.Join(" ", "exec", "--force-interactive", containerName, "--", "/sbin/simlog", "--uid",
                uid, "--gid", gid, shell)
        };

        var process = Process.Start(processInfo);
        await process?.WaitForExitAsync(token)!;
    }
}