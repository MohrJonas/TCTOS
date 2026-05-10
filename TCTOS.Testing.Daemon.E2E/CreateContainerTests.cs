using FluentAssertions;
using TCTOS.Abstractions.Data;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Testing.Daemon.E2E;

public class CreateContainerTests : TestBase
{
    [Fact]
    public async Task CreateContainer_CreatesContainer()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var createResponse = await socket.WriteAsync(new CreateContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName,
            Description = TestHelper.DefaultContainerDescription,
            Color = new Color
            {
                Red = 0,
                Green = 0,
                Blue = 0
            },
            Features = [],
            Image = TestHelper.DefaultImageName
        });
        createResponse.Error.Should().BeNull();
        var listResponse = await socket.WriteAsync<Container[]>(new ListContainersSocketMessage());
        listResponse.Error.Should().BeNull();
        listResponse.Data.Should().ContainSingle();
    }
    
    [Fact]
    public async Task CreateContainer_InvalidName_Errors()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var createResponse = await socket.WriteAsync(new CreateContainerSocketMessage
        {
            ContainerName = "-C$!nta in_r-",
            Description = TestHelper.DefaultContainerDescription,
            Color = new Color
            {
                Red = 0,
                Green = 0,
                Blue = 0
            },
            Features = [],
            Image = TestHelper.DefaultImageName
        });
        createResponse.Error.Should().Contain("is invalid");
    }
    
    [Fact]
    public async Task CreateContainer_ContainerWithNameAlreadyExists_Errors()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var firstCreateResponse = await socket.WriteAsync(new CreateContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName,
            Description = TestHelper.DefaultContainerDescription,
            Color = new Color
            {
                Red = 0,
                Green = 0,
                Blue = 0
            },
            Features = [],
            Image = TestHelper.DefaultImageName
        });
        firstCreateResponse.Error.Should().BeNull();
        var secondCreateResponse = await socket.WriteAsync(new CreateContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName,
            Description = TestHelper.DefaultContainerDescription,
            Color = new Color
            {
                Red = 0,
                Green = 0,
                Blue = 0
            },
            Features = [],
            Image = TestHelper.DefaultImageName
        });
        secondCreateResponse.Error.Should().Contain("already exists");
    }
}