using FluentAssertions;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Testing.Daemon.E2E;

public class ProvisionContainerTests : TestBase
{
    [Fact]
    public async Task SetProvisionContent_ContainerDoesNotExist_Errors()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new SetProvisionContentSocketMessage
        {
            Content = "foobar",
            ContainerName = TestHelper.DefaultContainerName
        });
        response.Error.Should().Contain("does not exist");
    }

    [Fact]
    public async Task SetProvisionContent_SetsContent()
    {
        await TestHelper.CreateContainer(IncusClient);

        const string provisionContent = "This is the new file content!";
        
        var socket = new UnixSocketWriter(SocketPath);
        var setResponse = await socket.WriteAsync(new SetProvisionContentSocketMessage
        {
            Content = provisionContent,
            ContainerName = TestHelper.DefaultContainerName
        });
        setResponse.Error.Should().BeNull();
        
        var getResponse = await socket.WriteAsync(new GetProvisionContentSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName
        });
        getResponse.Error.Should().BeNull();
        getResponse.Data.Should().Be(provisionContent);
    }
    
    [Fact]
    public async Task GetProvisionContent_ContainerDoesNotExist_Errors()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new GetProvisionContentSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName
        });
        response.Error.Should().Contain("does not exist");
    }
}