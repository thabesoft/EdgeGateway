namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class SerialTransportFactory : ITransportFactory
{
    public bool CanCreate(ITransportDescriptor descriptor)
    {
        return descriptor is SerialTransportDescriptor;
    }

    public ITransportConnection Create(ITransportDescriptor descriptor)
    {
        if (descriptor is not SerialTransportDescriptor serial)
            throw new InvalidOperationException();

        return new SerialTransportConnection(serial);
    }
}