using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Subscription;

internal sealed class SubscriptionValueReader<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
        IProtocolDescriptor protocolDescriptor,
        IByteDuplex byteDuplex,
        ProtocolAccessorFactorySelector protocolAccessorFactorySelector
    ) : IAsyncDisposable
    where T : notnull
{
    private readonly IProtocolAccessor _accessor = protocolAccessorFactorySelector.Create(protocolDescriptor, byteDuplex);


    public ValueTask<T> WaitNextAsync(CancellationToken cancellationToken = default)
    {
        return _accessor.ReadAsync(subscriptionDescriptor.Tag, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_accessor is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_accessor is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}