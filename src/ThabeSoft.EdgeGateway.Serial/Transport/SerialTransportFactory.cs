using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class SerialTransportFactory(ILoggerFactory loggerFactory) : ITransportConnectionFactory
{
    public bool CanCreate(TransportDescriptor descriptor)
    {
        return descriptor is SerialTransportDescriptor;
    }

    public ITransportConnection Create(TransportDescriptor descriptor)
    {
        if (descriptor is not SerialTransportDescriptor serial)
            throw new InvalidOperationException();

        return new SerialTransportConnection(
            descriptor: serial,
            stateMachine: Lifecycle.StrictStateMachine());
    }
}