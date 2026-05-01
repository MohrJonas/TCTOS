using TCTOS.Impls.Incus.DTOs;
using TCTOS.Impls.Incus.Response;

namespace TCTOS.Abstractions;

public interface IIncusClient
{
    public Task<ResponseBase> StartContainerAsync(string containerName);
    public Task<ResponseBase> StopContainerAsync(string containerName);
    public Task<ResponseBase<Instance[]>> GetContainersAsync();
    public Task<ResponseBase<Image[]>> GetImagesAsync();
    public Task<ResponseBase> UpdateContainerAsync(string containerName, InstancesPost changes);
    public Task<ResponseBase> CreateContainerAsync(InstancesPost containerParameters);
    public Task<ResponseBase<Instance>> GetContainerAsync(string containerName);
    public Task<ResponseBase<Image>> GetImageAsync(string fingerprint);
    public Task<ResponseBase> WaitForOperationAsync(string operationUrl);
}