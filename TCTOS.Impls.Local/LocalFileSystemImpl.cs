using System.Text.Json;
using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class LocalFileSystemImpl(ILogger<LocalFileSystemImpl> logger, string persistentRootPath) : IFileSystem
{
    public Task<Result<string?>> GetProvisioningFileContentAsync(string containerName)
    {
        return RunCatchingAsync(async () =>
        {
            using (logger.BeginScope("Getting container provisioning file content"))
            {
                logger.LogDebug("Container name is {name}", containerName);
                var path = PathHelper.GetPerContainerProvisioningFilePath(persistentRootPath, containerName);
                logger.LogDebug("Path is {path}", path);
                if (!File.Exists(path))
                {
                    logger.LogTrace("File does not exist, returning null");
                    return null;
                }

                var content = await File.ReadAllTextAsync(path);
                logger.LogDebug("Content is {content}", content);
                return content;
            }
        });
    }

    public Task<Result> SetProvisioningFileContentAsync(string containerName, string fileContent)
    {
        return RunCatchingAsync(async () =>
        {
            using (logger.BeginScope("Setting container provisioning file content"))
            {
                logger.LogDebug("Container name is {name}", containerName);
                logger.LogDebug("Content is {content}", fileContent);
                var path = PathHelper.GetPerContainerProvisioningFilePath(persistentRootPath, containerName);
                logger.LogDebug("Path is {path}", path);
                CreateParentDirectoriesForFile(path);
                await IoHelper.WriteTextFileAsync(path, fileContent, IoHelper.DefaultFileMode);
            }
        });
    }

    public Task<Result<ContainerConfiguration?>> GetContainerConfigurationAsync(string containerName)
    {
        return RunCatchingAsync(async () =>
        {
            using (logger.BeginScope("Getting container configuration file"))
            {
                logger.LogDebug("Container name is {name}", containerName);
                var path = PathHelper.GetPerContainerConfigurationPath(persistentRootPath, containerName);
                logger.LogDebug("Path is {path}", path);
                if (!File.Exists(path))
                {
                    logger.LogTrace("File does not exist, returning null");
                    return null;
                }

                var configuration = await ReadAndParse<ContainerConfiguration>(path);
                logger.LogDebug("Configuration is {configuration}", configuration);
                return configuration;
            }
        });
    }

    public Task<Result> SetContainerConfigurationAsync(string containerName, ContainerConfiguration configuration)
    {
        return RunCatchingAsync(async () =>
        {
            using (logger.BeginScope("Setting container configuration file"))
            {
                logger.LogDebug("Container name is {name}", containerName);
                logger.LogDebug("Configuration is {configuration}", configuration);
                var path = PathHelper.GetPerContainerConfigurationPath(persistentRootPath, containerName);
                logger.LogDebug("Path is {path}", path);
                CreateParentDirectoriesForFile(path);
                await UnparseAndWrite(path, configuration);
            }
        });
    }

    public Task<Result<TctOsConfiguration?>> GetConfigurationAsync()
    {
        return RunCatchingAsync(async () =>
        {
            using (logger.BeginScope("Getting configuration file"))
            {
                var path = PathHelper.GetConfigurationPath(persistentRootPath);
                logger.LogDebug("Path is {path}", path);
                if (!File.Exists(path))
                {
                    logger.LogTrace("File does not exist, returning null");
                    return null;
                }

                var configuration = await ReadAndParse<TctOsConfiguration>(path);
                logger.LogDebug("Configuration is {configuration}", configuration);
                return configuration;
            }
        });
    }

    public Task<Result> SetConfigurationAsync(TctOsConfiguration configuration)
    {
        return RunCatchingAsync(async () =>
        {
            logger.LogDebug("Configuration is {configuration}", configuration);
            var path = PathHelper.GetConfigurationPath(persistentRootPath);
            logger.LogDebug("Path is {path}", path);
            CreateParentDirectoriesForFile(path);
            await UnparseAndWrite(path, configuration);
        });
    }

    public Task<Result> RemoveContainerFiles(string containerName) => RunCatchingAsync(async () =>
    {
        var directoryPath = PathHelper.GetPerContainerRootPath(containerName);
        if(Directory.Exists(directoryPath))
            Directory.Delete(directoryPath, true);
    });

    private static void CreateParentDirectoriesForFile(string filePath)
    {
        var parentDirectory = Directory.GetParent(filePath);
        if(parentDirectory == null)
            return;
        IoHelper.CreateDirectory(parentDirectory.FullName, IoHelper.DefaultDirectoryMode);
    }

    private static async Task<TData> ReadAndParse<TData>(string path)
    {
        var text = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<TData>(text)!;
    }

    private static async Task UnparseAndWrite<TData>(string path, TData data)
    {
        var text = JsonSerializer.Serialize(data);
        await IoHelper.WriteTextFileAsync(path, text, IoHelper.DefaultFileMode);
    }
}