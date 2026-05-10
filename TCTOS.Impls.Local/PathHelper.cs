namespace TCTOS.Impls.Local;

internal static class PathHelper
{
    public static string GetPerContainerRootPath(string persistentRootPath)
    {
        return Path.Combine(persistentRootPath, "containers");
    }

    public static string GetPerContainerPath(string persistentRootPath, string containerName)
    {
        return Path.Combine(GetPerContainerRootPath(persistentRootPath), containerName);
    }

    public static string GetPerContainerProvisioningFilePath(string persistentRootPath, string containerName)
    {
        return Path.Combine(GetPerContainerPath(persistentRootPath, containerName), "provision");
    }

    public static string GetPerContainerConfigurationPath(string persistentRootPath, string containerName)
    {
        return Path.Combine(GetPerContainerPath(persistentRootPath, containerName), "configuration.json");
    }

    public static string GetPerContainerMountPath(string persistentRootPath, string containerName)
    {
        return Path.Combine(GetPerContainerPath(persistentRootPath, containerName), "mount");
    }

    public static string GetDesktopFilesRootPath(string persistentRootPath)
    {
        return Path.Combine(persistentRootPath, "applications");
    }
    
    public static string GetDesktopFilePath(string persistentRootPath, string desktopFileName)
    {
        return Path.Combine(GetDesktopFilesRootPath(persistentRootPath), desktopFileName);
    }

    public static string GetIconFileRootPath(string persistentRootPath, string containerName)
    {
        return Path.Combine(GetPerContainerPath(persistentRootPath, containerName), "icons");
    }
    
    public static string GetIconFilePath(string persistentRootPath, string containerName, string iconFileName)
    {
        return Path.Combine(GetIconFileRootPath(persistentRootPath, containerName), iconFileName);
    }
    
    public static string GetConfigurationPath(string persistentRootPath)
    {
        return Path.Combine(persistentRootPath, "configuration.json");
    }

    public static string GetFeaturesRootPath(string persistentRootPath)
    {
        return Path.Combine(persistentRootPath, "features");
    }

    public static string GetFeatureRootPath(string persistentRootPath, string featureName)
    {
        return Path.Combine(GetFeaturesRootPath(persistentRootPath), featureName);
    }

    public static string GetFeatureDescriptorPath(string persistentRootPath, string featureName)
    {
        return Path.Combine(GetFeatureRootPath(persistentRootPath, featureName), "descriptor.json");
    }

    public static string GetFeatureExecutablePath(string persistentRootPath, string featureName)
    {
        return Path.Combine(GetFeatureRootPath(persistentRootPath, featureName), "feature");
    }
    
    public static string GetTemporaryPath(string nonPersistentRootPath)
        => Path.Combine(nonPersistentRootPath, Guid.NewGuid().ToString());
}