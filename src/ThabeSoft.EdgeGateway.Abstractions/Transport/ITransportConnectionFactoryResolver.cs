
namespace ThabeSoft.EdgeGateway.Transports;

public interface ITransportConnectionFactoryResolver
{
    ITransportConnection Resolve(TransportDescriptor transportDescriptor);
}