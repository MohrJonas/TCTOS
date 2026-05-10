using FluentAssertions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Testing.Daemon.E2E;

public class ListContainersTests : TestBase
{
    [Fact]
    public async Task istContainers_NoContainers_ReturnsEmptyArray()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync<Container[]>(new ListContainersSocketMessage());
        response.Error.Should().BeNull();
        
        response.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task ListContainers_ContainersExist_ReturnsContainer()
    {
        await TestHelper.CreateContainer(IncusClient);
            
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync<Container[]>(new ListContainersSocketMessage());
        response.Error.Should().BeNull();
        
        response.Data.Should().ContainSingle(container =>
            container.ContainerName == TestHelper.DefaultContainerName
            && container.Description == TestHelper.DefaultContainerDescription
        );
    }
}