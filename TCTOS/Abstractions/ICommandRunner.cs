using TCTOS.Util;

namespace TCTOS.Abstractions;

public sealed record CommandResult(string? Stdout, string? Stderr, int? ExitCode);

public interface ICommandRunner
{
    public Task<Result<CommandResult>> RunCommand(string command, string[]? args = null, string? stdin = null,
        Dictionary<string, string>? env = null, string? cwd = null);

    public Task<Result<int?>> RunCommandInteractively(string command, string[]? args = null,
        Dictionary<string, string>? env = null, string? cwd = null);
}