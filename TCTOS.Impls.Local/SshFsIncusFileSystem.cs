using TCTOS.Abstractions;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class SshFsIncusFileSystem(IBackgroundCommandRunner backgroundCommandRunner, string persistentRootPath) : IIncusFileSystem
{
    private object? _backgroundJobIdentifier;
    private string? _fsRoot;

    public Task<Result> PrepareFileSystem(string containerName)
    {
        return RunCatchingAsync(async () =>
        {
            var mountPath = PathHelper.GetPerContainerMountPath(persistentRootPath, containerName);
            if(!Directory.Exists(mountPath))
                IoHelper.CreateDirectory(mountPath, IoHelper.DefaultDirectoryMode);
            _backgroundJobIdentifier = (await backgroundCommandRunner.RunCommandInBackground(
                "incus",
                ["file", "mount", $"{containerName}/", mountPath]
            )).GetOrThrow();
            _fsRoot = mountPath;
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

    public Task<Result<byte[]?>> GetIconBytesAsync(string iconName) => RunCatchingAsync<byte[]?>(async () =>
    {
        const string hicolorThemeDirectory = "/usr/share/icons/hicolor";
        var indexFilePath = Path.Combine(hicolorThemeDirectory, "index.theme");
        if (!(await DoesFileExistAsync(indexFilePath)).GetOrThrow())
            return null;
        var indexFileContent = (await GetFileTextAsync(indexFilePath)).GetOrThrow();
        var indexFile = ThemeFileParser.ParseThemeFileFromText(indexFileContent);

        var folderIconPaths = indexFile["Icon Theme"]["Directories"]
            .Split(',')
            .Where(path => indexFile[path]["Context"] == "Applications")
            .OrderByDescending(path =>
            {
                var size = int.Parse(indexFile[path]["Size"]);
                var scale = indexFile[path].ContainsKey("Scale")
                    ? int.Parse(indexFile[path]["Scale"])
                    : 1;
                return size / scale;
            })
            .Select(path => Path.Combine(hicolorThemeDirectory, path));

        foreach (var folderIconPath in folderIconPaths)
        {
            var filesInDirectory = (await ListFilesAsync(folderIconPath)).GetOrThrow();
            foreach (var file in filesInDirectory)
            {
                if (Path.GetFileNameWithoutExtension(file) == iconName)
                    return (await GetFileAsync(file)).GetOrThrow();
            }
        }
        
        return null;
    });

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