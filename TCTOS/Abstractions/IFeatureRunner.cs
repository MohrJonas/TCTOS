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
        IBackgroundCommandRunner backgroundCommandRunner,
        IDictionary<string, string> env);

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