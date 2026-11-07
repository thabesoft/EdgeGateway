namespace ThabeSoft.EdgeGateway.Transport;


public interface ITransportConnectionFactory
{
    bool CanCreate(ITransportDescriptor transportDescriptor);
    ITransportConnection Create(ITransportDescriptor transportDescriptor);
}