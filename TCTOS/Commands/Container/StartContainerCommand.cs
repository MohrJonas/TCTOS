using System.CommandLine;
using TCTOS.IOC;

namespace TCTOS.Commands.Container;

public sealed class StartContainerCommand(DiContainer container) 
    : CommandBase("start", "Start the container", container, ["up"])
{
    protected override Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}