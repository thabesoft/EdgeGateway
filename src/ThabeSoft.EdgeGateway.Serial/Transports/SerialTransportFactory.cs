using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

namespace ThabeSoft.EdgeGateway.Transports;


internal sealed class SerialTransportFactory(
        ILoggerFactory loggerFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory
    ) : ITransportConnectionResolver
{
    public bool CanResolve(TransportCreationContext context)
    {
        return context.Transport is SerialTransportDescriptor;
    }

    public ITransportConnection Resolve(TransportCreationContext context)
    {
        if (context.Transport is not SerialTransportDescriptor serial)
            throw new InvalidOperationException();

        var logger = loggerFactory.CreateLogger<SerialTransportConnection>();
        var stateMachine = lifecycleStateMachineFactory.Create(context.StateMachine);

        return new SerialTransportConnection(descriptor: serial, stateMachine: stateMachine, logger)
        {
            DisposedCallback = async () => await stateMachine.TryDisposeAsync(logger)
        };
    }
}