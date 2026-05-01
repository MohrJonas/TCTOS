using System.Diagnostics;
using TCTOS.Abstractions;
using TCTOS.Util;

namespace TCTOS.Impls;

public sealed class SystemdBackgroundCommandRunner : IBackgroundCommandRunner
{
    public Task<Result<object?>> RunCommandInBackground(string command, string[]? args = null, string? stdin = null,
        Dictionary<string, string>? env = null, string? cwd = null) => RunCatchingAsync<object?>(async () =>
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "systemd-run",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        var unitName = Guid.NewGuid().ToString();
        List<string> commandArgs = [
            "--user",
            "--unit",
            unitName,
            command
        ];
        
        if (args != null)
            commandArgs.AddRange(args);
        
        startInfo.Arguments = string.Join(" ", commandArgs);
        
        if (env != null)
            foreach (var (key, value) in env)
                startInfo.Environment.Add(key, value);
        if (cwd != null)
            startInfo.WorkingDirectory = cwd;

        var process = Process.Start(startInfo);
        if (process == null)
            return null;

        if (stdin != null)
        {
            await process.StandardInput.WriteAsync(stdin);
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();
        }
            
        await process.WaitForExitAsync();
        return unitName;
    });

    public Task<Result> StopBackgroundCommand(object identifier) => RunCatchingAsync(async () =>
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "systemctl",
            Arguments = string.Join(" ", ["--user", "stop", identifier.ToString()]),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        var process = Process.Start(startInfo);

        if (process != null)
            await process.WaitForExitAsync();
    });
}