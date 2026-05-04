using System.Text.Json;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class LocalFileSystemFeatureProvider : IFeatureProvider
{
    public Task<Result<FeatureDescriptor[]>> GetAvailableFeaturesAsync()
    {
        return RunCatchingAsync(async () =>
        {
            var root = LocalFileSystemImpl.PathHelper.GetFeaturesRootPath();
            return await Task.WhenAll(Directory.GetDirectories(root)
                .Select(async path =>
                {
                    var featureName = Path.GetFileName(path);
                    var descriptorPath = LocalFileSystemImpl.PathHelper.GetFeatureDescriptorPath(featureName);
                    var contents = await File.ReadAllTextAsync(descriptorPath);
                    return JsonSerializer.Deserialize<FeatureDescriptor>(contents)!;
                }));
        });
    }

    public Task<Result<string>> GetFeatureScriptTextAsync(string featureName)
    {
        return RunCatchingAsync(async () =>
            await File.ReadAllTextAsync(LocalFileSystemImpl.PathHelper.GetFeatureExecutablePath(featureName))
        );
    }
}