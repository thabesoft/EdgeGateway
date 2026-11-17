using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

namespace ThabeSoft.EdgeGateway.Transports;


internal sealed class SerialTransportFactory(
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

        // 那我怎么拿到状态机描述符
        var stateMachine = lifecycleStateMachineFactory.Create(descriptor.StateMachine);

        return new SerialTransportConnection(descriptor: serial, stateMachine: stateMachine, logger)
        {
            DisposedCallback = async () => await stateMachine.TryDisposeAsync(logger)
        };
    }
}