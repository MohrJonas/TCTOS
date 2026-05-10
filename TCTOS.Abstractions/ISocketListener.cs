namespace TCTOS.Abstractions;

public interface ISocketListener
{
    public event SocketEventHandler? OnMessage;
    public Task ListenAsync(CancellationToken cancellationToken);
}