using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : IStatefulLifecycle, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }

    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }
    IDevice AddDevice(DeviceDescriptor deviceDescriptor);
}