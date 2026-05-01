using System.CommandLine;
using TCTOS.IOC;

namespace TCTOS.Commands.Container;

public sealed class ListContainersCommand(DiContainer container)
    : CommandBase("list", "List all containers", container, ["ls"])
{
    protected override Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}