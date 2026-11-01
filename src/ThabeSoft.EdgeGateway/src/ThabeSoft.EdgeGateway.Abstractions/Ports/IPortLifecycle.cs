namespace ThabeSoft.EdgeGateway.Ports;

public interface IPortLifecycle
{
    bool IsOpen { get; }

    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);
}
