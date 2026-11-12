using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceFactory(
    SubscriptionFactory subscriptionFactory,
    ProtocolAccessorFactorySelector protocolAccessorFactorySelector,
    ILoggerFactory loggerFactory)
{
    public Device Create(DeviceDescriptor deviceDescriptor, ILifecycleStateable portLifecycleState, IByteDuplex byteDuplex)
    {
        return new
        (
            deviceDescriptor: deviceDescriptor,
            subscriptionFactory: subscriptionFactory,
            byteDuplex: byteDuplex,
            portLifecycleState: portLifecycleState,
            protocolAccessorFactorySelector: protocolAccessorFactorySelector,
            logger: loggerFactory.CreateLogger<Device>()
        );
    }
}