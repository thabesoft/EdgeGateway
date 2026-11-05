using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : ILifecycle, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }

    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }
    ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor deviceDescriptor);
}