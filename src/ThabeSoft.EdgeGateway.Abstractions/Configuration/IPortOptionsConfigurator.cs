using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IPortOptionsConfigurator
{
    IPortOptionsConfigurator WithTransport(ITransportDescriptor transportDescriptor);
    IPortOptionsConfigurator AddDevice(Action<IDeviceOptionsConfigurator> buildActnion);
}