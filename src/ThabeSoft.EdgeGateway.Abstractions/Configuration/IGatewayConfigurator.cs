using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IGatewayConfigurator
{
    IPortConfigurator AddPort(ITransportDescriptor transport);
}

public delegate void GatewayConfiguratorHandler(IGatewayConfigurator configurator);