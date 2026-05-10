using System.CommandLine;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;
using TCTOS.Impls.Local;

namespace TCTOS.Console.Commands;

public static class LaunchCommandArguments
{
    public static readonly Argument<string> ExecutableNameArgument = new("executable")
    {
        Description = "The executable to run"
    };
}

public sealed class LaunchCommand()
    : CommandBase("launch", "Launch the executable in the specified container",
        arguments: [SharedArguments.ContainerNameArgument, LaunchCommandArguments.ExecutableNameArgument])
{
    protected override async Task RunAsync(ParseResult parseResult, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var executable = parseResult.GetRequiredValue(LaunchCommandArguments.ExecutableNameArgument);
        var executableArguments = parseResult.UnmatchedTokens;

        var socketPath = parseResult.GetRequiredValue(SharedOptions.SocketPathOption);
        
        var writer = new UnixSocketWriter(socketPath);

        var userInformationCollector = new LocalUserInformationCollector();
        
        var response = await writer.WriteAsync(new LaunchSocketMessage
        {
            ContainerName = containerName,
            Command = executable,
            Args = executableArguments.ToArray(),
            Gid = userInformationCollector.GetGid(),
            Uid = userInformationCollector.GetUid()
        });
        
        response.ExitOnError();
    }
}