using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionValueReaderFactory(ProtocolAccessorFactorySelector protocolAccessorFactorySelector)
{
    public SubscriptionValueReader<T> Create<T>(SubscriptionDescriptor<T> subscriptionDescriptor, IProtocolDescriptor protocolDescriptor, IByteDuplex byteDuplex) where T : notnull
    {
        return new
        (
            subscriptionDescriptor: subscriptionDescriptor,
            protocolDescriptor: protocolDescriptor,
            byteDuplex: byteDuplex,
            protocolAccessorFactorySelector: protocolAccessorFactorySelector
        );
    }
}
