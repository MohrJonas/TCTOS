using TCTOS.Common;

namespace TCTOS.Features.Abstractions;

public interface IFeature
{
    public Task<Result<DescribedValue<bool>>> IsApplicable(string containerName, FeatureContext featureContext);
    public Task<Result> Apply(string containerName, FeatureContext featureContext);
    public Task<Result> Unapply(string containerName, FeatureContext featureContext);
}