using EdgeGateway.Ports;

namespace EdgeGateway.Options.Monitor;


public interface IGatewayOptionsMonitor
{
    GatewayOptions? CurrentOptions { get; }


    IObservable<IPortOptions> PortAdded { get; }
    IObservable<IPortOptions> PortRemoved { get; }
}