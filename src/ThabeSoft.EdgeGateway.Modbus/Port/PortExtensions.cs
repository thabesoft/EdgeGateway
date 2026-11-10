using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Port;


public static class PortExtensions
{
    extension(IPort port)
    {
        public ValueTask<IDevice> AddModbusRtuDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default) =>
            port.AddDeviceAsync(deviceId, Modbus.Rtu, cancellationToken);
        public ValueTask<IDevice> AddModbusRtuDeviceAsync(CancellationToken cancellationToken = default) =>
            port.AddModbusRtuDeviceAsync(DeviceId.Create(), cancellationToken);


        public ValueTask<IDevice> AddModbusAsciiDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default) =>
           port.AddDeviceAsync(deviceId, Modbus.Ascii, cancellationToken);
        public ValueTask<IDevice> AddModbusAsciiDeviceAsync(CancellationToken cancellationToken = default) =>
            port.AddModbusAsciiDeviceAsync(DeviceId.Create(), cancellationToken);


        public ValueTask<IDevice> AddModbusIpDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default) =>
           port.AddDeviceAsync(deviceId, Modbus.Ip, cancellationToken);
        public ValueTask<IDevice> AddModbusIpDeviceAsync(CancellationToken cancellationToken = default) =>
            port.AddModbusIpDeviceAsync(DeviceId.Create(), cancellationToken);
    }
}
