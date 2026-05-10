using System.CommandLine;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Console.Commands.Container.Feature;

public static class RemoveFeatureCommandArguments
{
    public static readonly Argument<string> FeatureNameArgument = new("feature_name")
    {
        Description = "Name of the feature to remove"
    };
}

public sealed class RemoveFeatureCommand()
    : CommandBase("remove", "Remove the specified feature from the container",
        arguments: [SharedArguments.ContainerNameArgument, RemoveFeatureCommandArguments.FeatureNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var featureName = parseResult.GetRequiredValue(RemoveFeatureCommandArguments.FeatureNameArgument);
        
        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var response = await writer.WriteAsync(new RemoveFeatureSocketMessage
        {
            ContainerName = containerName,
            FeatureName = featureName
        });
        
        response.ExitOnError();
    }
}