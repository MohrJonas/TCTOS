using System.CommandLine;
using TCTOS.Console.Abstractions;
using TCTOS.Console.IOC;

namespace TCTOS.Console.Commands.Container;

public sealed class ListContainersCommand(DiContainer container)
    : CommandBase("list", "List all containers", container, ["ls"])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var client = container.Get<IIncusClient>();
        var containers = (await client.GetContainersAsync()).Metadata;
        var nonPersistentStorage = container.Get<INonPersistentStorage>();
        foreach (var instance in containers)
        {
            var key = $"{instance.Name}-enabled-features";
            var enabledFeatures = nonPersistentStorage.PeekValue<string[]>(key);
            System.Console.WriteLine(
                $"{instance.Name}\t{instance.Description}\t{instance.Status}\t{string.Join(", ", enabledFeatures)}");
        }
    }
}