using System.Reflection;
using TCTOS.Abstractions;
using TCTOS.Common;
using TCTOS.Features.Abstractions;

namespace TCTOS.Impls.Local;

public sealed class CSharpFeatureRunner : IFeatureRunner
{
    private static IFeature GetFeatureFromBase64Assembly(string base64String)
    {
        var assemblyBytes = Convert.FromBase64String(base64String);
        var assembly = Assembly.Load(assemblyBytes);
        var featureType = assembly.ExportedTypes.Single(type =>
        {
            var implementedInterfaces = type.GetInterfaces();
            return implementedInterfaces.Contains(typeof(IFeature));
        });
        return (IFeature)Activator.CreateInstance(featureType)!;
    }
    
    public async Task<DescribedValue<bool>> CanApplyFeature(string featureScriptText, string containerName, IFileSystem fileSystem, IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage, IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider, ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner)
    {
        var featureInstance = GetFeatureFromBase64Assembly(featureScriptText);
        var isApplicableResult = await featureInstance.IsApplicable(containerName, new FeatureContext
        {
            FileSystem = fileSystem,
            IncusClient = incusClient,
            NonPersistentStorage = nonPersistentStorage,
            UserInformationCollector = userInformationCollector,
            EnvironmentVariableProvider = environmentVariableProvider,
            BackgroundCommandRunner = backgroundCommandRunner,
            CommandRunner = commandRunner,
            ContainerEnvironmentVariables = new Dictionary<string, string>()
        });
        if (isApplicableResult.HasFailed)
            return new DescribedValue<bool>(false,
                $"Applicability check failed with exception {isApplicableResult.Exception!.Message}");
        return isApplicableResult.GetOrThrow();
    }

    public async Task<Result> ApplyFeature(string featureScriptText, string containerName, IFileSystem fileSystem, IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage, IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider, ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner, IDictionary<string, string> env)
    {
        var featureInstance = GetFeatureFromBase64Assembly(featureScriptText);
        return await featureInstance.Apply(containerName, new FeatureContext
        {
            FileSystem = fileSystem,
            IncusClient = incusClient,
            NonPersistentStorage = nonPersistentStorage,
            UserInformationCollector = userInformationCollector,
            EnvironmentVariableProvider = environmentVariableProvider,
            BackgroundCommandRunner = backgroundCommandRunner,
            CommandRunner = commandRunner,
            ContainerEnvironmentVariables = env
        });
    }

    public async Task<Result> UnapplyFeature(string featureScriptText, string containerName, IFileSystem fileSystem, IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage, IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider, ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner)
    {
        var featureInstance = GetFeatureFromBase64Assembly(featureScriptText);
        return await featureInstance.Unapply(containerName, new FeatureContext
        {
            FileSystem = fileSystem,
            IncusClient = incusClient,
            NonPersistentStorage = nonPersistentStorage,
            UserInformationCollector = userInformationCollector,
            EnvironmentVariableProvider = environmentVariableProvider,
            BackgroundCommandRunner = backgroundCommandRunner,
            CommandRunner = commandRunner,
            ContainerEnvironmentVariables = new Dictionary<string, string>()
        });
    }
}