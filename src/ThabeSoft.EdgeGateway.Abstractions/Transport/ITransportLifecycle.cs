namespace ThabeSoft.EdgeGateway.Transport;

public interface ITransportLifecycle
{
    bool IsOpen { get; }

    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);
}


public enum TransportState
{
    Closed,
    Opening,
    Open,
    Closing,
    Faulted
}