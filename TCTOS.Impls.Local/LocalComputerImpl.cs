using TCTOS.Abstractions;
using TCTOS.Abstractions.Data.DesktopFiles;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class LocalComputerImpl(string persistentRootPath) : IComputer
{
    public Task<Result> AddDesktopFileAsync(string name, DesktopFile desktopFile) => RunCatchingAsync(async () =>
    {
        var desktopFilePath = PathHelper.GetDesktopFilePath(persistentRootPath, name);
        var parentDirectory = Directory.GetParent(desktopFilePath);
        if(parentDirectory != null)
            IoHelper.CreateDirectory(parentDirectory.FullName, IoHelper.DefaultDirectoryMode);
        await IoHelper.WriteTextFileAsync(desktopFilePath, desktopFile.SerializeToString(), IoHelper.DefaultFileMode);
    });

    public Task<Result<string>> AddIconFileAsync(string name, string containerName, byte[] imageBytes) => RunCatchingAsync(async () =>
    {
        var iconFilePath = PathHelper.GetIconFilePath(persistentRootPath, containerName, name);
        var parentDirectory = Directory.GetParent(iconFilePath);
        if(parentDirectory != null)
            IoHelper.CreateDirectory(parentDirectory.FullName, IoHelper.DefaultDirectoryMode);
        await IoHelper.WriteBinaryFileAsync(iconFilePath, imageBytes, IoHelper.DefaultFileMode);
        return iconFilePath;
    });

    public Task<Result> RemoveDesktopFileAsync(string name) => RunCatchingAsync(() =>
    {
        try
        {
            var desktopFilePath = PathHelper.GetDesktopFilePath(persistentRootPath, name);
            File.Delete(desktopFilePath);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    });

    public Task<Result> RemoveIconFileAsync(string name, string containerName) => RunCatchingAsync(() =>
    {
        try
        {
            var iconFilePath = PathHelper.GetIconFilePath(persistentRootPath, containerName, name);
            File.Delete(iconFilePath);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    });

    public Task<Result> RemoveContainerFiles(string containerName) => RunCatchingAsync(() =>
    {
        try
        {
            var iconDirectoryPath = PathHelper.GetIconFileRootPath(persistentRootPath, containerName);
            if(Directory.Exists(iconDirectoryPath))
                Directory.Delete(iconDirectoryPath, true);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    });

    public Task<Result<string[]>> ListDesktopFilesAsync(string containerName) => RunCatchingAsync(async () =>
    {
        var desktopFilesDirectory = PathHelper.GetDesktopFilesRootPath(containerName);
        if (!Directory.Exists(desktopFilesDirectory))
            return [];
        return Directory.GetFiles(desktopFilesDirectory)
            .Where(p => Path.GetFileNameWithoutExtension(p).StartsWith(containerName))
            .ToArray();
    });
}