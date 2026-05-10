using FluentAssertions;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Testing.Daemon.E2E;

public class StopContainerTests : TestBase
{
    [Fact]
    public async Task StopContainer_ContainerDoesNotExists_Errors()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new StopContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName
        });
        response.Error.Should().Contain("does not exist");
    }

    [Fact]
    public async Task StopContainer_ContainerAlreadyStopped_Ignores()
    {
        await TestHelper.CreateContainer(IncusClient);
        
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new StopContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName
        });
        response.Error.Should().BeNull();
    }

    [Fact]
    public async Task StopContainer_ContainerRunning_StopsContainer()
    {
        await TestHelper.CreateContainer(IncusClient);

        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new StopContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName
            
        });
        response.Error.Should().BeNull();

        (await IncusClient.GetContainerAsync(TestHelper.DefaultContainerName)).Metadata.Status.Should().Be("Stopped");
    }
}