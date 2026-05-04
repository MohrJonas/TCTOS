using System.Diagnostics;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class LocalCommandRunnerImpl : ICommandRunner
{
    public Task<Result<CommandResult>> RunCommand(string command, string[]? args = null, string? stdin = null,
        Dictionary<string, string>? env = null, string? cwd = null)
    {
        return RunCatchingAsync(async () =>
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            if (args != null)
                startInfo.Arguments = string.Join(" ", args);
            if (env != null)
                foreach (var (key, value) in env)
                    startInfo.Environment.Add(key, value);
            if (cwd != null)
                startInfo.WorkingDirectory = cwd;

            var process = Process.Start(startInfo);
            if (process == null)
                return new CommandResult(null, null, null);

            if (stdin != null)
            {
                await process.StandardInput.WriteAsync(stdin);
                await process.StandardInput.FlushAsync();
                process.StandardInput.Close();
            }

            await process.WaitForExitAsync();
            return new CommandResult(
                await process.StandardOutput.ReadToEndAsync(),
                await process.StandardError.ReadToEndAsync(),
                process.ExitCode
            );
        });
    }

    public Task<Result<int?>> RunCommandInteractively(string command, string[]? args = null,
        Dictionary<string, string>? env = null, string? cwd = null)
    {
        return RunCatchingAsync<int?>(async () =>
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command
            };
            if (args != null)
                startInfo.Arguments = string.Join(" ", args);
            if (env != null)
                foreach (var (key, value) in env)
                    startInfo.Environment.Add(key, value);
            if (cwd != null)
                startInfo.WorkingDirectory = cwd;

            var process = Process.Start(startInfo);
            if (process == null)
                return null;

            await process.WaitForExitAsync();
            return process.ExitCode;
        });
    }
}