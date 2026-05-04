using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Abstractions;

public interface IFeatureProvider
{
    public Task<Result<FeatureDescriptor[]>> GetAvailableFeaturesAsync();
    public Task<Result<string>> GetFeatureScriptTextAsync(string featureName);
}