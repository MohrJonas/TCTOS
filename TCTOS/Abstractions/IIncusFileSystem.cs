using TCTOS.Util;

namespace TCTOS.Abstractions;

public interface IIncusFileSystem
{
    public Task<Result> PrepareFileSystem(string containerName);
    public Task<Result> DisposeFileSystem(string containerName);
    public Task<Result<string[]>> ListFilesAsync(string containerPath);
    public Task<Result<string>> GetFileTextAsync(string containerPath);
    public Task<Result<byte[]>> GetFileAsync(string containerPath);
    public Task<Result<bool>> DoesFileExistAsync(string containerPath);
    public Task<Result<bool>> DoesDirectoryExistAsync(string containerPath);
}