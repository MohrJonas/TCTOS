using TCTOS.Abstractions;
using TCTOS.Abstractions.Data.DesktopFiles;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class LocalComputerImpl : IComputer
{
    public Task<Result> AddDesktopFileAsync(string name, DesktopFile desktopFile) => RunCatchingAsync(async () =>
    {
        var parentDirectory =
            Path.Combine(Environment.GetEnvironmentVariable("HOME")!, ".tctos", "share", "applications");
        Directory.CreateDirectory(parentDirectory);
        var filePath = Path.Combine(parentDirectory, name);
        await File.WriteAllTextAsync(filePath, desktopFile.SerializeToString());
    });

    public Task<Result<string>> AddIconFileAsync(string name, byte[] imageBytes) => RunCatchingAsync(async () =>
    {
        var iconDirectory = Path.Combine(Environment.GetEnvironmentVariable("HOME")!, ".tctos", "icons");
        Directory.CreateDirectory(iconDirectory);
        var iconPath = Path.Combine(iconDirectory, name);
        await File.WriteAllBytesAsync(iconPath, imageBytes);
        return iconPath;
    });

    public Task<Result> RemoveDesktopFileAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RemoveIconFileAsync(string name)
    {
        throw new NotImplementedException();
    }
}