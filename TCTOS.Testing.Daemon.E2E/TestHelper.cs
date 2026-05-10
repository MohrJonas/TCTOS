using TCTOS.Abstractions;
using TCTOS.Abstractions.Incus.Data;
using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Abstractions.Incus.DTOs;

namespace TCTOS.Testing.Daemon.E2E;

public static class TestHelper
{
    public const string DefaultContainerName = "containerName";
    public const string DefaultContainerDescription = "containerDescription";
    public const string DefaultImageName = "ubuntu/questing";
    public const string DefaultPoolName = "default";
    public const string DefaultBridgeName = "incusbr0";
    
    public static async Task RemoveAllContainers(IIncusClient incusClient)
    {
        var instances = (await incusClient.GetContainersAsync()).Metadata;
        foreach (var instance in instances)
        {
            if (instance.Status == "Running")
            {
                var stopResult = await incusClient.StopContainerAsync(instance.Name);
                stopResult.ThrowOnError();
                (await incusClient.WaitForOperationAsync(stopResult.Operation!)).ThrowOnError();
            }
            var deleteResult = await incusClient.DeleteContainerAsync(instance.Name);
            deleteResult.ThrowOnError();
            (await incusClient.WaitForOperationAsync(deleteResult.Operation!)).ThrowOnError();
        }
    }

    public static async Task CreateContainer(IIncusClient client, Action<InstancesPost>? postEditor = null)
    {
       
        var post = new InstancesPost
        {
            Name = DefaultContainerName,
            Description = DefaultContainerDescription,
            Devices = new Dictionary<string, object>
            {
                {
                    "root", new IncusDiskDevice
                    {
                        Path = "/",
                        Pool = DefaultPoolName
                    }
                },
                {
                    "net", new IncusNicDevice
                    {
                        Name = "eth0",
                        Network = DefaultBridgeName
                    }
                }
            },
            Source = new InstanceSource
            {
                Type = ImageSourceType.Image,
                Alias = DefaultImageName,
                Server = "https://images.linuxcontainers.org",
                Protocol = "simplestreams",
                Mode = TransferMode.Pull
            },
            Type = "container",
            Profiles = []
        };
        postEditor?.Invoke(post);
        var createResponse = await client.CreateContainerAsync(post);
        createResponse.ThrowOnError();
        (await client.WaitForOperationAsync(createResponse.Operation!)).ThrowOnError();
    }
}