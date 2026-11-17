
namespace ThabeSoft.EdgeGateway.Transports;

public interface ITransportConnectionFactoryFactory
{
    ITransportConnection Resolve(TransportDescriptor transportDescriptor);
}