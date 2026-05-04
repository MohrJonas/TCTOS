using System.CommandLine;
using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Operations;

namespace TCTOS.Console.Commands;

public static class LaunchCommandArguments
{
    public static readonly Argument<string> ExecutableNameArgument = new("executable")
    {
        Description = "The executable to run"
    };
}

public sealed class LaunchCommand(DiContainer container)
    : CommandBase("launch", "Launch the executable in the specified container", container,
        arguments: [SharedArguments.ContainerNameArgument, LaunchCommandArguments.ExecutableNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var executable = parseResult.GetRequiredValue(LaunchCommandArguments.ExecutableNameArgument);
        var executableArguments = parseResult.UnmatchedTokens;
        
        var logger = container.Get<ILogger>();
        var incusClient = container.Get<IIncusClient>();
        var userInformationCollector = container.Get<IUserInformationCollector>();
        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        var featureProvider = container.Get<IFeatureProvider>();
        var featureRunner = container.Get<IFeatureRunner>();
        var fileSystem = container.Get<IFileSystem>();
        var variableProvider = container.Get<IEnvironmentVariableProvider>();
        var runner = container.Get<ICommandRunner>();
        var backgroundRunner = container.Get<IBackgroundCommandRunner>();
        
        (await LaunchApplicationOperation.LaunchApplicationAsync(
            logger, 
            containerName, 
            [executable, ..executableArguments], 
            incusClient, 
            userInformationCollector, 
            nonPersistentStorage, 
            featureProvider, 
            featureRunner, 
            fileSystem, 
            variableProvider, 
            runner, 
            backgroundRunner    
        )).ThrowIfFailed();
    }
}