using System.Text.Json;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Impls.Local;

public sealed class DllFeatureProvider(string persistentRootPath) : IFeatureProvider
{
    public Task<Result<FeatureDescriptor[]>> GetAvailableFeaturesAsync() => RunCatchingAsync(async () =>
    {
        var root = PathHelper.GetFeaturesRootPath(persistentRootPath);
        return await Task.WhenAll(Directory.GetDirectories(root)
            .Select(async path =>
            {
                var featureName = Path.GetFileName(path);
                var descriptorPath = PathHelper.GetFeatureDescriptorPath(persistentRootPath, featureName);
                var contents = await File.ReadAllTextAsync(descriptorPath);
                return JsonSerializer.Deserialize<FeatureDescriptor>(contents)!;
            }));
    });

    public Task<Result<string>> GetFeatureScriptTextAsync(string featureName) => RunCatchingAsync(async () =>
        Convert.ToBase64String(
            await File.ReadAllBytesAsync(Path.ChangeExtension(PathHelper.GetFeatureExecutablePath(persistentRootPath, featureName), ".dll"))
        )
    );
}