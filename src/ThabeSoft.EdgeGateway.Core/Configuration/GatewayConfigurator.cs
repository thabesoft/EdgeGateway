using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


internal class GatewayConfigurator(IGateway gateway) :
    IGatewayConfigurator
{
    public IPortConfigurator AddPort(ITransportDescriptor transport)
    {
        var port = gateway.AddPort(transport);
        return new PortConfigurator(gateway, port);
    }
}
