using System.CommandLine;
using TCTOS.IOC;

namespace TCTOS.Commands.Container;

public sealed class StopContainerCommand(DiContainer container) 
    : CommandBase("stop", "Stop the container", container, ["halt", "down"])
{
    protected override Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}