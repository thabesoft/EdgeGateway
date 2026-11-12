using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Subscription;

internal sealed class SubscriptionValueReader<T>(
        SubscriptionDescriptor<T> subscriptionDescriptor,
        IProtocolAccessor protocolAccessor
    ) where T : notnull
{
    public ValueTask<T> WaitNextAsync(CancellationToken cancellationToken = default)
    {
        return protocolAccessor.ReadAsync(subscriptionDescriptor.Tag, cancellationToken);
    }
}