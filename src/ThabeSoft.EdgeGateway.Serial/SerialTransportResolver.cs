using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Serial;


internal sealed class SerialTransportResolver(
        ILoggerFactory loggerFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory
    ) : ITransportConnectionResolver
{
    public bool CanResolve(TransportDescriptor descriptor)
    {
        return descriptor is SerialTransportDescriptor;
    }

    public ITransportConnection Resolve(TransportDescriptor descriptor)
    {
        if (descriptor is not SerialTransportDescriptor serial)
            throw new InvalidOperationException();

        var logger = loggerFactory.CreateLogger<SerialTransportConnection>();
        var stateMachine = lifecycleStateMachineFactory.Create(descriptor.StateMachine);

        return new SerialTransportConnection(descriptor: serial, stateMachine: stateMachine, logger)
        {
            DisposedCallback = async () => await stateMachine.TryDisposeAsync(logger)
        };
    }
}