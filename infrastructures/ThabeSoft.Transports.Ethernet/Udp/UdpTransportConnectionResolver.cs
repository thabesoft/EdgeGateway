using Microsoft.Extensions.Logging;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Transports;
using ThabeSoft.Primitives;

namespace ThabeSoft.Transports.Ethernet.Udp;


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

        return new UdpTransportConnection
        (
           ip: ip.IP,
           port: ip.Port,
           endpoint: ip.Endpoint,
           stateMachine: stateMachine,
           logger: logger, onDispose: () => Safe.DisposeSafelyAsync(stateMachine, logger)
        );
    }
}
