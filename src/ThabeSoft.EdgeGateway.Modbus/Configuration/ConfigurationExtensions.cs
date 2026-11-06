using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class ConfigurationExtensions
{
    extension(IPortConfigurator configurator)
    {
        public IDeviceConfigurator AddModbusRtuDevice(DeviceId deviceId) =>
            configurator.AddDevice(deviceId, Modbus.Rtu);
        public IDeviceConfigurator AddModbusRtuDevice() =>
            configurator.AddModbusRtuDevice(DeviceId.Create());


        public IDeviceConfigurator AddModbusAsciiDevice(DeviceId deviceId) =>
           configurator.AddDevice(deviceId, Modbus.Ascii);
        public IDeviceConfigurator AddModbusAsciiDevice() =>
            configurator.AddModbusAsciiDevice(DeviceId.Create());


        public IDeviceConfigurator AddModbusIpDevice(DeviceId deviceId) =>
           configurator.AddDevice(deviceId, Modbus.Rtu);
        public IDeviceConfigurator AddModbusIpDevice() =>
            configurator.AddModbusIpDevice(DeviceId.Create());
    }
}
