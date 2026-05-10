using TCTOS.Abstractions.Data.DesktopFiles;
using TCTOS.Common;

namespace TCTOS.Abstractions;

public interface IComputer
{
    public Task<Result> AddDesktopFileAsync(string name, DesktopFile desktopFile);
    public Task<Result<string>> AddIconFileAsync(string name, string containerName, byte[] imageBytes);
    public Task<Result> RemoveDesktopFileAsync(string name);
    public Task<Result> RemoveIconFileAsync(string name, string containerName);
    Task<Result> RemoveContainerFiles(string containerName);
    public Task<Result<string[]>> ListDesktopFilesAsync(string containerName);
}