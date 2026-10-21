using EdgeGateway.Ports;

namespace EdgeGateway.Options.Monitor;


public interface IGatewayOptionsMonitor
{
    GatewayDescriptor? CurrentOptions { get; }


    IObservable<IPortOptions> PortAdded { get; }
    IObservable<IPortOptions> PortRemoved { get; }
}