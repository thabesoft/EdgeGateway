namespace ThabeSoft.EdgeGateway.Transport;


public interface ITransportConnectionFactory
{
    bool CanCreate(TransportDescriptor transportDescriptor);
    ITransportConnection Create(TransportDescriptor transportDescriptor);
}