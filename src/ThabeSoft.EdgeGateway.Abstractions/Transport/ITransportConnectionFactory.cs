namespace ThabeSoft.EdgeGateway.Transports;


public interface ITransportConnectionFactory
{
    bool CanCreate(TransportDescriptor transportDescriptor);
    ITransportConnection Create(TransportDescriptor transportDescriptor);
}