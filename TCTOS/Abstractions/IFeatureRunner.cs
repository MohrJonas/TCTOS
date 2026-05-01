namespace TCTOS.Abstractions;

public interface IFeatureRunner
{
    public Task<bool> CanApplyFeature(
        string featureScriptText,
        string containerName,
        IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage,
        IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider,
        ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner);

    public Task ApplyFeature(
        string featureScriptText,
        string containerName,
        IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage,
        IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider,
        ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner);

    public Task UnapplyFeature(
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