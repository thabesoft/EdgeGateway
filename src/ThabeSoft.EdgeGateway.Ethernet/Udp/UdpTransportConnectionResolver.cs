using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Ethernet.Udp;


internal sealed class UdpTransportConnectionResolver(
        ILoggerFactory loggerFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory
    ) : ITransportConnectionResolver
{
    public bool CanResolve(TransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.ProtocolType == IpProtocolType.Udp;
    }

    public ITransportConnection Resolve(TransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.ProtocolType != IpProtocolType.Udp)
            throw new InvalidOperationException();

        var logger = loggerFactory.CreateLogger<UdpTransportConnection>();
        var stateMachine = lifecycleStateMachineFactory.Create(descriptor.StateMachine);

        return new UdpTransportConnection(ip, stateMachine, logger)
        {
            DisposedCallback = async () => await stateMachine.TryDisposeAsync(logger)
        };
    }
}
