using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Ethernet;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

namespace ThabeSoft.EdgeGateway.Transports.Udp;


internal sealed class UdpTransportConnectionResolver(
        ILoggerFactory loggerFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory
    ) : ITransportConnectionResolver
{
    public bool CanResolve(TransportCreationContext context)
    {
        return context.Transport is IpTransportDescriptor ip && ip.ProtocolType == IpProtocolType.Udp;
    }

    public ITransportConnection Resolve(TransportCreationContext context)
    {
        if (context.Transport is not IpTransportDescriptor ip || ip.ProtocolType != IpProtocolType.Udp)
            throw new InvalidOperationException();

        var logger = loggerFactory.CreateLogger<UdpTransportConnection>();
        var stateMachine = lifecycleStateMachineFactory.Create(context.StateMachine);

        return new UdpTransportConnection(ip, stateMachine, logger)
        {
            DisposedCallback = async () => await stateMachine.TryDisposeAsync(logger)
        };
    }
}
