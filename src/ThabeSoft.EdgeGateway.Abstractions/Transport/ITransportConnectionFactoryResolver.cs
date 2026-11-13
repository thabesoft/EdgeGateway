
namespace ThabeSoft.EdgeGateway.Transport;

public interface ITransportConnectionFactoryResolver
{
    ITransportConnection Resolve(ITransportDescriptor transportDescriptor);
}