
namespace ThabeSoft.EdgeGateway.Transport;

public interface ITransportConnectionFactoryResolver
{
    ITransportConnection Resolve(TransportDescriptor transportDescriptor);
}