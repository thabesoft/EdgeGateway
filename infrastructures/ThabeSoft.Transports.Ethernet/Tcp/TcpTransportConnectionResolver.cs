using Microsoft.Extensions.Logging;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Transports;

namespace ThabeSoft.Transports.Ethernet.Tcp;


internal sealed class TcpTransportConnectionResolver(
        ILoggerFactory loggerFactory,
        ILifecycleFactory lifecycleFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory
    ) : ITransportConnectionResolver
{
    public bool CanResolve(TransportDescriptor descriptor)
    {
        return descriptor is IpTransportDescriptor ip && ip.ProtocolType == IpProtocolType.Tcp;
    }

    public ITransportConnection Resolve(TransportDescriptor descriptor)
    {
        if (descriptor is not IpTransportDescriptor ip || ip.ProtocolType != IpProtocolType.Tcp)
            throw new InvalidOperationException();

        var logger = loggerFactory.CreateLogger<TcpTransportConnection>();
        return new TcpTransportConnection(ip, lifecycleFactory, lifecycleStateMachineFactory, logger);
    }
}
