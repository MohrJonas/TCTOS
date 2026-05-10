using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Incus.Data;
using TCTOS.Abstractions.Incus.Devices;
using TCTOS.Abstractions.Incus.DTOs;
using TCTOS.Common;
using TCTOS.Operations.Exceptions;

namespace TCTOS.Operations;

public static class CreateContainerOperation
{
    public static Task<Result> CreateContainerAsync(
        string containerName,
        Color accentColor,
        string[] enabledFeatures,
        string? containerDescription,
        string imageName,
        ILogger logger,
        IIncusClient incusClient,
        IContainerProvisioner containerProvisioner,
        IFeatureProvider featureProvider,
        IFileSystem fileSystem
    ) => RunCatchingAsync(async () =>
    {
        if (!NameValidator.IsValidContainerName(containerName))
        {
            logger.LogWarning("Container name {name} is invalid", containerName);
            throw new InvalidContainerNameException(containerName);
        }
            
        var existingContainerNames
            = (await incusClient.GetContainersAsync()).Metadata.Select(static i => i.Name);

        if (existingContainerNames.Contains(containerName))
            throw new ContainerAlreadyExistsException(containerName);

        var existingFeatureNames = (await featureProvider.GetAvailableFeaturesAsync())
            .GetOrThrow()
            .Select(d => d.Name)
            .ToArray();

        foreach (var featureName in enabledFeatures)
            if (!existingFeatureNames.Contains(featureName))
                throw new Exception($"Feature with name \"{featureName}\" does not exist");
        
        var globalConfiguration = (await fileSystem.GetConfigurationAsync()).GetOrThrow();

        if (globalConfiguration == null)
            throw new Exception("No global configuration found");

        var response = await incusClient.CreateContainerAsync(new InstancesPost
        {
            Name = containerName,
            Description = containerDescription,
            Devices = new Dictionary<string, object>
            {
                {
                    "root", new IncusDiskDevice
                    {
                        Path = "/",
                        Pool = globalConfiguration.PoolName
                    }
                },
                {
                    "net", new IncusNicDevice
                    {
                        Name = "eth0",
                        Network = globalConfiguration.BridgeName
                    }
                }
            },
            Source = new InstanceSource
            {
                Type = ImageSourceType.Image,
                Alias = imageName,
                Server = "https://images.linuxcontainers.org",
                Protocol = "simplestreams",
                Mode = TransferMode.Pull
            },
            Config = new Dictionary<string, object>
            {
                { "security.nesting", true },
                //{ "security.protection.delete", true },
                { "security.guestapi", false }
            },
            Type = "container",
            Profiles = []
        });

        response.ThrowOnError();

        (await incusClient.WaitForOperationAsync(response.Operation!)).ThrowOnError();
        
        (await fileSystem.SetProvisioningFileContentAsync(containerName,
            containerProvisioner.GetDefaultProvisionFileTemplate())).ThrowIfFailed();

        (await fileSystem.SetContainerConfigurationAsync(containerName, new ContainerConfiguration
        {
            FeatureNames = enabledFeatures,
            Color = accentColor
        })).ThrowIfFailed();
    });
}