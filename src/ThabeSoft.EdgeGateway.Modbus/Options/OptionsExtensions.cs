using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class OptionsExtensions
{
    extension(IPortOptionsConfigurator builder)
    {
        public IPortOptionsConfigurator AddModbusRtuDevice(DeviceId deviceId, Action<IDeviceOptionsConfigurator>? buildAction = null) =>
            builder.AddDevice(x => { buildAction?.Invoke(x); x.UseModbusRtu(deviceId); });
        public IPortOptionsConfigurator AddModbusRtuDevice(Action<IDeviceOptionsConfigurator>? buildAction = null) =>
            builder.AddModbusRtuDevice(DeviceId.Create(), buildAction);


        public IPortOptionsConfigurator AddModbusAsciiDevice(DeviceId deviceId, Action<IDeviceOptionsConfigurator>? buildAction = null) =>
           builder.AddDevice(x => { buildAction?.Invoke(x); x.UseModbusAscii(deviceId); });
        public IPortOptionsConfigurator AddModbusAsciiDevice(Action<IDeviceOptionsConfigurator>? buildAction = null) =>
            builder.AddModbusAsciiDevice(DeviceId.Create(), buildAction);


        public IPortOptionsConfigurator AddModbusIpDevice(DeviceId deviceId, Action<IDeviceOptionsConfigurator>? buildAction = null) =>
           builder.AddDevice(x => { buildAction?.Invoke(x); x.UseModbusIp(deviceId); });
        public IPortOptionsConfigurator AddModbusIpDevice(Action<IDeviceOptionsConfigurator>? buildAction = null) =>
            builder.AddModbusIpDevice(DeviceId.Create(), buildAction);
    }

    extension(IDeviceOptionsConfigurator builder)
    {
        public IDeviceOptionsConfigurator UseModbusRtu(DeviceId deviceId) =>
            builder.UseProtocol(deviceId, ModbusProtocolDescriptor.Rtu);
        public IDeviceOptionsConfigurator UseModbusRtu() =>
            builder.UseModbusRtu(DeviceId.Create());


        public IDeviceOptionsConfigurator UseModbusAscii(DeviceId deviceId) =>
            builder.UseProtocol(deviceId, ModbusProtocolDescriptor.Ascii);
        public IDeviceOptionsConfigurator UseModbusAscii() =>
            builder.UseModbusAscii(DeviceId.Create());


        public IDeviceOptionsConfigurator UseModbusIp(DeviceId deviceId) =>
            builder.UseProtocol(deviceId, ModbusProtocolDescriptor.Ip);
        public IDeviceOptionsConfigurator UseModbusIp() =>
            builder.UseModbusIp(DeviceId.Create());
    }
}
