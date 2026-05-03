using TCTOS.Impls.Incus.DTOs;
using TCTOS.Impls.Incus.Response;

namespace TCTOS.Abstractions;

public interface IIncusClient
{
    public Task<ResponseBase> StartContainerAsync(string containerName);
    public Task<ResponseBase> StopContainerAsync(string containerName);
    public Task<ResponseBase<Instance[]>> GetContainersAsync();
    public Task<ResponseBase<string[]>> GetContainerNamesAsync();
    public Task<ResponseBase<Image[]>> GetImagesAsync();
    public Task<ResponseBase> UpdateContainerPartiallyAsync(string containerName, InstancePut changes);
    public Task<ResponseBase> UpdateContainerAsync(string containerName, InstancePut changes);
    public Task<ResponseBase> CreateContainerAsync(InstancesPost containerParameters);
    public Task<ResponseBase<Instance>> GetContainerAsync(string containerName);
    public Task<ResponseBase<Image>> GetImageAsync(string fingerprint);
    public Task<ResponseBase> WaitForOperationAsync(string operationUrl);

    public Task<ResponseBase> RunCommand(
        string containerName,
        string command,
        string[]? args = null,
        Dictionary<string, object>? env = null,
        string? cwd = null
    );
}