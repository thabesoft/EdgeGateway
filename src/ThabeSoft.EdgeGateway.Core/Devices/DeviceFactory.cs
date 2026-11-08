using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceFactory(
    SubscriptionFactory subscriptionFactory,
    ILoggerFactory loggerFactory)
{
    public Device Create(DeviceDescriptor deviceDescriptor, ILifecycleState portLifecycleState, IByteDuplex byteDuplex)
    {
        return new
        (
            deviceDescriptor: deviceDescriptor,
            subscriptionFactory: subscriptionFactory,
            portLifecycleState: portLifecycleState,
            byteDuplex: byteDuplex,
            logger: loggerFactory.CreateLogger<Device>()
        );
    }
}