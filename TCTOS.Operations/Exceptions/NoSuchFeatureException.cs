namespace TCTOS.Operations.Exceptions;

public sealed class NoSuchFeatureException(string featureName)
    : Exception($"Feature \"{featureName}\" does not exist");