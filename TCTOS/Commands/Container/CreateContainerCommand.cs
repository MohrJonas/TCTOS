using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.Data;
using TCTOS.Impls.Incus.Data;
using TCTOS.Impls.Incus.Devices;
using TCTOS.Impls.Incus.DTOs;
using TCTOS.IOC;

namespace TCTOS.Commands.Container;

public static class CreateContainerCommandOptions
{
    public static readonly Option<string> ContainerDescriptionOption = new("--description", "-d")
    {
        Description = "Description of the container",
        DefaultValueFactory = _ => string.Empty
    };

    public static readonly Option<string[]> ContainerFeaturesOption = new("--feature", "-f")
    {
        Description = "Enable the given feature for the container",
        DefaultValueFactory = _ => []
    };
}

public static class CreateContainerCommandArguments
{
    public static readonly Argument<string> ImageNameArgument = new("image_name")
    {
        Description = "Name of the image to use"
    };
}

public sealed class CreateContainerCommand(DiContainer container)
    : CommandBase("create", "Create a new container", container, aliases: ["new"],
        arguments: [SharedArguments.ContainerNameArgument, CreateContainerCommandArguments.ImageNameArgument],
        options:
        [
            CreateContainerCommandOptions.ContainerDescriptionOption,
            CreateContainerCommandOptions.ContainerFeaturesOption
        ])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var mangledContainerName = NameMangler.MangleContainerName(containerName);

        var client = container.Get<IIncusClient>();

        var existingContainerNames
            = (await client.GetContainersAsync()).Metadata.Select(static i => i.Name);

        if (existingContainerNames.Contains(mangledContainerName))
            throw new Exception(
                $"Container with the name \"{mangledContainerName}\" ({containerName} before mangle) already exists");

        var featureProvider = container.Get<IFeatureProvider>();
        var existingFeatureNames = (await featureProvider.GetAvailableFeaturesAsync())
            .GetOrThrow()
            .Select(d => d.Name)
            .ToArray();

        foreach (var featureName in parseResult.GetRequiredValue(CreateContainerCommandOptions.ContainerFeaturesOption))
        {
            if (!existingFeatureNames.Contains(featureName))
                throw new Exception($"Feature with name \"{featureName}\" does not exist");
        }

        var fileSystem = container.Get<IFileSystem>();

        var globalConfiguration = (await fileSystem.GetConfigurationAsync()).GetOrThrow();

        if (globalConfiguration == null)
            throw new Exception("No global configuration found");

        var response = (await client.CreateContainerAsync(new InstancesPost()
        {
            Name = mangledContainerName,
            Description = parseResult.GetRequiredValue(CreateContainerCommandOptions.ContainerDescriptionOption),
            Devices = new Dictionary<string, object>()
            {
                {
                    "root", new IncusDiskDevice()
                    {
                        Path = "/",
                        Pool = globalConfiguration.PoolName
                    }
                },
                {
                    "net", new IncusNicDevice()
                    {
                        Name = "eth0",
                        Network = globalConfiguration.BridgeName
                    }
                }
            },
            Source = new InstanceSource()
            {
                Type = ImageSourceType.Image,
                Alias = "ubuntu/questing/default/amd64",
                Server = "https://images.linuxcontainers.org",
                Protocol = "simplestreams",
                Mode = TransferMode.Pull
            },
            Config = new Dictionary<string, object>()
            {
                { "security.nesting", true }
            },
            Type = "container",
            Profiles = []
        }));

        response.ThrowOnError();

        (await client.WaitForOperationAsync(response.Operation!)).ThrowOnError();

        var provisioner = container.Get<IContainerProvisioner>();

        (await fileSystem.SetProvisioningFileContentAsync(containerName,
            provisioner.GetDefaultProvisionFileTemplate())).ThrowIfFailed();

        (await fileSystem.SetContainerConfigurationAsync(containerName, new ContainerConfiguration()
        {
            FeatureNames = parseResult.GetRequiredValue(CreateContainerCommandOptions.ContainerFeaturesOption)
        })).ThrowIfFailed();
    }
}