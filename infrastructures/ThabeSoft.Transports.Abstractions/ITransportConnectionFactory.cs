namespace ThabeSoft.EdgeGateway.Transports;


public interface ITransportConnectionFactory
{
    ITransportConnection Create(TransportDescriptor descriptor);
}