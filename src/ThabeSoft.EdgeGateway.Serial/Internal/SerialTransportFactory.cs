using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Serial.Internal;


internal sealed class SerialTransportFactory(ILoggerFactory loggerFactory) : ITransportConnectionFactory
{
    public bool CanCreate(ITransportDescriptor descriptor)
    {
        return descriptor is SerialTransportDescriptor;
    }

    public ITransportConnection Create(ITransportDescriptor descriptor)
    {
        if (descriptor is not SerialTransportDescriptor serial)
            throw new InvalidOperationException();

        return new SerialTransportConnection(serial, loggerFactory.CreateLogger<SerialTransportConnection>());
    }
}