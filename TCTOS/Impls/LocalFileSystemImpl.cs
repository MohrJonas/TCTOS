using System.Text.Json;
using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Data;
using TCTOS.Util;

namespace TCTOS.Impls;

public sealed class LocalFileSystemImpl(ILogger<LocalFileSystemImpl> logger) : IFileSystem
{
    public Task<Result<string?>> GetProvisioningFileContentAsync(string containerName)
    {
        return RunCatchingAsync(async () =>
        {
            using (logger.BeginScope("Getting container provisioning file content"))
            {
                logger.LogDebug("Container name is {name}", containerName);
                var path = PathHelper.GetPerContainerProvisioningFilePath(containerName);
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
                var path = PathHelper.GetPerContainerProvisioningFilePath(containerName);
                logger.LogDebug("Path is {path}", path);
                CreateParentDirectoriesForFile(path);
                await File.WriteAllTextAsync(path, fileContent);
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
                var path = PathHelper.GetPerContainerConfigurationPath(containerName);
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
                var path = PathHelper.GetPerContainerConfigurationPath(containerName);
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
                var path = PathHelper.GetConfigurationPath();
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
            var path = PathHelper.GetConfigurationPath();
            logger.LogDebug("Path is {path}", path);
            CreateParentDirectoriesForFile(path);
            await UnparseAndWrite(path, configuration);
        });
    }

    private static void CreateParentDirectoriesForFile(string filePath)
    {
        Directory.CreateDirectory(Directory.GetParent(filePath)!.FullName);
    }

    private static async Task<TData> ReadAndParse<TData>(string path)
    {
        var text = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<TData>(text)!;
    }

    private static async Task UnparseAndWrite<TData>(string path, TData data)
    {
        var text = JsonSerializer.Serialize(data);
        await File.WriteAllTextAsync(path, text);
    }

    public static class PathHelper
    {
        private const string RootPath = "/tmp/TCtOs";

        public static string GetPerContainerRootPath()
        {
            return Path.Combine(RootPath, "containers");
        }

        public static string GetPerContainerPath(string containerName)
        {
            return Path.Combine(GetPerContainerRootPath(), containerName);
        }

        public static string GetPerContainerProvisioningFilePath(string containerName)
        {
            return Path.Combine(GetPerContainerPath(containerName), "provision");
        }

        public static string GetPerContainerConfigurationPath(string containerName)
        {
            return Path.Combine(GetPerContainerPath(containerName), "configuration.json");
        }

        public static string GetConfigurationPath()
        {
            return Path.Combine(RootPath, "configuration.json");
        }

        public static string GetFeaturesRootPath()
        {
            return Path.Combine(RootPath, "features");
        }

        public static string GetFeatureRootPath(string featureName)
        {
            return Path.Combine(GetFeaturesRootPath(), featureName);
        }

        public static string GetFeatureDescriptorPath(string featureName)
        {
            return Path.Combine(GetFeatureRootPath(featureName), "descriptor.json");
        }

        public static string GetFeatureExecutablePath(string featureName)
        {
            return Path.Combine(GetFeatureRootPath(featureName), "feature");
        }
    }
}