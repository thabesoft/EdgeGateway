using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

namespace ThabeSoft.EdgeGateway.Transports.Tcp;


internal sealed class TcpTransportConnectionResolver(
        ILoggerFactory loggerFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory
    ) : ITransportConnectionResolver
{
    public bool CanResolve(TransportCreationContext context)
    {
        return context.Transport is IpTransportDescriptor ip && ip.ProtocolType == IpProtocolType.Tcp;
    }

    public ITransportConnection Resolve(TransportCreationContext context)
    {
        if (context.Transport is not IpTransportDescriptor ip || ip.ProtocolType != IpProtocolType.Tcp)
            throw new InvalidOperationException();

        var logger = loggerFactory.CreateLogger<TcpTransportConnection>();
        var stateMachine = lifecycleStateMachineFactory.Create(context.StateMachine);

        return new TcpTransportConnection(ip, stateMachine, logger)
        {
            DisposedCallback = async () => await stateMachine.TryDisposeAsync(logger)
        };
    }
}
