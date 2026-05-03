using TCTOS.Util;

namespace TCTOS.Abstractions;

public interface IBackgroundCommandRunner
{
    public Task<Result<object?>> RunCommandInBackground(string command, string[]? args = null, string? stdin = null,
        Dictionary<string, string>? env = null, string? cwd = null);

    public Task<Result> StopBackgroundCommand(object identifier);
}