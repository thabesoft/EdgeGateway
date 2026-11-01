namespace ThabeSoft.EdgeGateway.Channels;

public interface IChannelLifecycle
{
    bool IsOpen { get; }

    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);
}