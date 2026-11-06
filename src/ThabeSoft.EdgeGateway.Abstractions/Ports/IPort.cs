using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : ILifecycle, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }

    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }
    IDevice AddDevice(DeviceDescriptor deviceDescriptor);
}

public static class PortExtensions
{
    extension(IPort port)
    {
        public IDevice AddDevice(DeviceId deviceId, IProtocolDescriptor protocolDescriptor)
        {
            ArgumentNullException.ThrowIfNull(deviceId);
            ArgumentNullException.ThrowIfNull(protocolDescriptor);


            return port.AddDevice(DeviceDescriptor.Create(deviceId, protocolDescriptor));
        }

        public IDevice AddDevice(IProtocolDescriptor protocolDescriptor) =>
            port.AddDevice(DeviceId.Create(), protocolDescriptor);
    }
}