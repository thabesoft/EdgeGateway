namespace ThabeSoft.Transports;


public interface ITransportConnectionFactory
{
    ITransportConnection Create(TransportDescriptor descriptor);
}