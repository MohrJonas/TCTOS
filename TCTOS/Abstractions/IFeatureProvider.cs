using TCTOS.Data;
using TCTOS.Util;

namespace TCTOS.Abstractions;

public interface IFeatureProvider
{
    public Task<Result<FeatureDescriptor[]>> GetAvailableFeaturesAsync();
    public Task<Result<string>> GetFeatureScriptTextAsync(string featureName);
}