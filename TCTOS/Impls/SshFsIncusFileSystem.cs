using TCTOS.Abstractions;
using TCTOS.Util;

namespace TCTOS.Impls;

public sealed class SshFsIncusFileSystem(IBackgroundCommandRunner backgroundCommandRunner) : IIncusFileSystem
{
    private object? _backgroundJobIdentifier;
    private string? _fsRoot;

    public Task<Result> PrepareFileSystem(string containerName)
    {
        return RunCatchingAsync(async () =>
        {
            var tempPath = Directory.CreateTempSubdirectory().FullName;
            _backgroundJobIdentifier = (await backgroundCommandRunner.RunCommandInBackground(
                "incus",
                ["file", "mount", $"{containerName}/", tempPath]
            )).GetOrThrow();
            _fsRoot = tempPath;
            await Task.Delay(500);
        });
    }

    public Task<Result> DisposeFileSystem(string containerName)
    {
        return RunCatchingAsync(async () =>
        {
            if (_backgroundJobIdentifier != null)
                (await backgroundCommandRunner.StopBackgroundCommand(_backgroundJobIdentifier)).ThrowIfFailed();
        });
    }

    public Task<Result<string[]>> ListFilesAsync(string containerPath)
    {
        return RunCatchingAsync(() =>
        {
            try
            {
                var path = PrependMountPointPrefix(containerPath);
                return Task.FromResult(Directory.GetFileSystemEntries(path).Select(RemoveMountPointPrefix).ToArray());
            }
            catch (Exception e)
            {
                return Task.FromException<string[]>(e);
            }
        });
    }

    public Task<Result<string>> GetFileTextAsync(string containerPath)
    {
        return RunCatchingAsync(async () =>
        {
            var path = PrependMountPointPrefix(containerPath);
            return await File.ReadAllTextAsync(path);
        });
    }

    public Task<Result<byte[]>> GetFileAsync(string containerPath)
    {
        return RunCatchingAsync(async () =>
        {
            var path = PrependMountPointPrefix(containerPath);
            return await File.ReadAllBytesAsync(path);
        });
    }

    public Task<Result<bool>> DoesFileExistAsync(string containerPath)
    {
        return RunCatchingAsync(() =>
        {
            try
            {
                var path = PrependMountPointPrefix(containerPath);
                return Task.FromResult(File.Exists(path));
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        });
    }

    public Task<Result<bool>> DoesDirectoryExistAsync(string containerPath)
    {
        return RunCatchingAsync(() =>
        {
            try
            {
                var path = PrependMountPointPrefix(containerPath);
                return Task.FromResult(Directory.Exists(path));
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        });
    }

    private string RemoveMountPointPrefix(string path)
    {
        return !path.StartsWith(_fsRoot!)
            ? throw new Exception($"Cannot trim fs root from path {path}, since it does not start with {_fsRoot}")
            : path[_fsRoot!.Length..];
    }

    private string PrependMountPointPrefix(string path)
    {
        return Path.Combine(_fsRoot!, path.TrimStart(Path.DirectorySeparatorChar));
    }
}