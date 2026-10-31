namespace ThabeSoft.EdgeGateway.Channel;

public interface IChannelLifecycle
{
    bool IsOpen { get; }

    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);
}