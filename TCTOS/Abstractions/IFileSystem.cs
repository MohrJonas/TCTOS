using TCTOS.Data;
using TCTOS.Util;

namespace TCTOS.Abstractions;

public interface IFileSystem
{
    public Task<Result<string?>> GetProvisioningFileContentAsync(string containerName);
    public Task<Result> SetProvisioningFileContentAsync(string containerName, string fileContent);

    public Task<Result<ContainerConfiguration?>> GetContainerConfigurationAsync(string containerName);
    public Task<Result> SetContainerConfigurationAsync(string containerName, ContainerConfiguration configuration);

    public Task<Result<TctOsConfiguration?>> GetConfigurationAsync();
    public Task<Result> SetConfigurationAsync(TctOsConfiguration configuration);
}