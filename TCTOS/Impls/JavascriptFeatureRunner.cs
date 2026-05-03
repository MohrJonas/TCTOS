using Jint;
using TCTOS.Abstractions;

namespace TCTOS.Impls;

public sealed class JavascriptFeatureRunner : IFeatureRunner
{
    public async Task<bool> CanApplyFeature(string featureScriptText, string containerName, IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage, IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider, ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner)
    {
        using var engine = new Engine(static configure =>
        {
            configure.ExperimentalFeatures = ExperimentalFeature.TaskInterop;
            configure.AllowClr(typeof(JavascriptFeatureRunner).Assembly);
        });
        engine
            .SetValue("fileSystem", fileSystem)
            .SetValue("incusClient", incusClient)
            .SetValue("nonPersistentStorage", nonPersistentStorage)
            .SetValue("userInformationCollector", userInformationCollector)
            .SetValue("environmentVariableProvider", environmentVariableProvider)
            .SetValue("commandRunner", commandRunner)
            .SetValue("containerName", containerName)
            .SetValue("backgroundCommandRunner", backgroundCommandRunner)
            .SetValue("debug", (Action<object>)Console.WriteLine);
        await engine.ExecuteAsync(featureScriptText);
        return (await engine.InvokeAsync("canApplyFeature")).AsBoolean();
    }

    public async Task ApplyFeature(string featureScriptText, string containerName, IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage, IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider, ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner, IDictionary<string, string> env)
    {
        using var engine = new Engine(static configure =>
        {
            configure.ExperimentalFeatures = ExperimentalFeature.TaskInterop;
            configure.AllowClr(typeof(JavascriptFeatureRunner).Assembly);
        });
        engine
            .SetValue("fileSystem", fileSystem)
            .SetValue("incusClient", incusClient)
            .SetValue("nonPersistentStorage", nonPersistentStorage)
            .SetValue("userInformationCollector", userInformationCollector)
            .SetValue("environmentVariableProvider", environmentVariableProvider)
            .SetValue("commandRunner", commandRunner)
            .SetValue("containerName", containerName)
            .SetValue("backgroundCommandRunner", backgroundCommandRunner)
            .SetValue("env", env)
            .SetValue("debug", (Action<object>)Console.WriteLine);
        await engine.ExecuteAsync(featureScriptText);
        await engine.InvokeAsync("applyFeature");
    }

    public async Task UnapplyFeature(string featureScriptText, string containerName, IFileSystem fileSystem,
        IIncusClient incusClient,
        INonPersistentStorage nonPersistentStorage, IUserInformationCollector userInformationCollector,
        IEnvironmentVariableProvider environmentVariableProvider, ICommandRunner commandRunner,
        IBackgroundCommandRunner backgroundCommandRunner)
    {
        using var engine = new Engine(static configure =>
        {
            configure.ExperimentalFeatures = ExperimentalFeature.TaskInterop;
            configure.AllowClr(typeof(JavascriptFeatureRunner).Assembly);
        });
        engine
            .SetValue("fileSystem", fileSystem)
            .SetValue("incusClient", incusClient)
            .SetValue("nonPersistentStorage", nonPersistentStorage)
            .SetValue("userInformationCollector", userInformationCollector)
            .SetValue("environmentVariableProvider", environmentVariableProvider)
            .SetValue("commandRunner", commandRunner)
            .SetValue("containerName", containerName)
            .SetValue("backgroundCommandRunner", backgroundCommandRunner)
            .SetValue("debug", (Action<object>)Console.WriteLine);
        await engine.ExecuteAsync(featureScriptText);
        await engine.InvokeAsync("unapplyFeature");
    }
}