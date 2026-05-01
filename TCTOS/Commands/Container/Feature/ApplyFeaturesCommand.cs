using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.IOC;

namespace TCTOS.Commands.Container.Feature;

public sealed class ApplyFeaturesCommand(DiContainer container) 
    : CommandBase("apply", "Apply the defined features to the container", container, arguments: [SharedArguments.ContainerNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);

        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();
        var client = container.Get<IIncusClient>();
        var variableProvider = container.Get<IEnvironmentVariableProvider>();
        var userInformationCollector = container.Get<IUserInformationCollector>();
        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        var runner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();
        
        var configuration = (await fileSystem.GetContainerConfigurationAsync(containerName)).GetOrThrow()!;

        foreach (var featureName in configuration.FeatureNames)
        {
            var featureText = (await featureProvider.GetFeatureScriptTextAsync(featureName)).GetOrThrow();
            await featureRunner.ApplyFeature(
                featureText,
                containerName,
                fileSystem,
                client,
                nonPersistentStorage,
                userInformationCollector,
                variableProvider,
                runner,
                backgroundRunner
            );
        }
    }
}