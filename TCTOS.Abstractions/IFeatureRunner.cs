using TCTOS.Common;

namespace TCTOS.Abstractions;

public interface IFeatureRunner
{
    public Task<DescribedValue<bool>> CanApplyFeature(
        string featureScriptText,
        string containerName,
        IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage,
        IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider,
        ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner);

    public Task<Result> ApplyFeature(
        string featureScriptText,
        string containerName,
        IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage,
        IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider,
        ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner,
        IDictionary<string, string> env);

    public Task<Result> UnapplyFeature(
        string featureScriptText,
        string containerName,
        IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage,
        IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider,
        ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner);
}