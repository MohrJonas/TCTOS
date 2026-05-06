using TCTOS.Abstractions.Data.DesktopFiles;
using TCTOS.Common;

namespace TCTOS.Abstractions;

public interface IComputer
{
    public Task<Result> AddDesktopFileAsync(string name, DesktopFile desktopFile);
    public Task<Result<string>> AddIconFileAsync(string name, byte[] imageBytes);
    public Task<Result> RemoveDesktopFileAsync(string name);
    public Task<Result> RemoveIconFileAsync(string name);
}