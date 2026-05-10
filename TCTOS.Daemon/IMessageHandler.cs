using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Daemon;

public interface IMessageHandler
{
    public Task<Result<SocketResponse>> HandleMessage(SocketMessage message);
}