using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Options;


public static class OptionsExtensions
{
    extension(IPortOptionsBuilder builder)
    {
        public IPortOptionsBuilder AddModbusRtuDevice(DeviceId deviceId, Action<IDeviceOptionsBuilder>? buildAction = null) =>
            builder.AddDevice(x => { buildAction?.Invoke(x); x.UseModbusRtu(deviceId); });
        public IPortOptionsBuilder AddModbusRtuDevice(Action<IDeviceOptionsBuilder>? buildAction = null) =>
            builder.AddModbusRtuDevice(DeviceId.Create(), buildAction);


        public IPortOptionsBuilder AddModbusAsciiDevice(DeviceId deviceId, Action<IDeviceOptionsBuilder>? buildAction = null) =>
           builder.AddDevice(x => { buildAction?.Invoke(x); x.UseModbusAscii(deviceId); });
        public IPortOptionsBuilder AddModbusAsciiDevice(Action<IDeviceOptionsBuilder>? buildAction = null) =>
            builder.AddModbusAsciiDevice(DeviceId.Create(), buildAction);


        public IPortOptionsBuilder AddModbusIpDevice(DeviceId deviceId, Action<IDeviceOptionsBuilder>? buildAction = null) =>
           builder.AddDevice(x => { buildAction?.Invoke(x); x.UseModbusIp(deviceId); });
        public IPortOptionsBuilder AddModbusIpDevice(Action<IDeviceOptionsBuilder>? buildAction = null) =>
            builder.AddModbusIpDevice(DeviceId.Create(), buildAction);
    }

    extension(IDeviceOptionsBuilder builder)
    {
        public IDeviceOptionsBuilder UseModbusRtu(DeviceId deviceId) =>
            builder.UseProtocol(deviceId, ModbusProtocolDescriptor.Rtu);
        public IDeviceOptionsBuilder UseModbusRtu() =>
            builder.UseModbusRtu(DeviceId.Create());


        public IDeviceOptionsBuilder UseModbusAscii(DeviceId deviceId) =>
            builder.UseProtocol(deviceId, ModbusProtocolDescriptor.Ascii);
        public IDeviceOptionsBuilder UseModbusAscii() =>
            builder.UseModbusAscii(DeviceId.Create());


        public IDeviceOptionsBuilder UseModbusIp(DeviceId deviceId) =>
            builder.UseProtocol(deviceId, ModbusProtocolDescriptor.Ip);
        public IDeviceOptionsBuilder UseModbusIp() =>
            builder.UseModbusIp(DeviceId.Create());
    }
}
