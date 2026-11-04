using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Options;

public interface IPortOptionsBuilder
{
    IPortOptionsBuilder WithTransport(ITransportDescriptor transportDescriptor);
    IPortOptionsBuilder AddDevice(Action<IDeviceOptionsBuilder> buildActnion);
}