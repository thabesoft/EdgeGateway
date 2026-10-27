using EdgeGateway.Config;
using EdgeGateway.Modbus;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class ConfigBuilderExtensions
{
    extension(PortConfigBuilder portConfigBuilder)
    {
        public PortConfigBuilder AddModbusRtuDevice()
        {
            var builder = DeviceConfig.Builder()
                .ModbusRtu();

            return portConfigBuilder.AddDevice(builder.Build());
        }

        public PortConfigBuilder AddModbusAsciiDevice()
        {
            var builder = DeviceConfig.Builder()
                .ModbusAscii();

            return portConfigBuilder.AddDevice(builder.Build());
        }

        public PortConfigBuilder AddModbusTcpDevice()
        {
            var builder = DeviceConfig.Builder()
                .ModbusTcp();

            return portConfigBuilder.AddDevice(builder.Build());
        }
    }

    extension(DeviceConfigBuilder deviceConfigBuilder)
    {
        public DeviceConfigBuilder ModbusAscii()
        {
            return deviceConfigBuilder.SetDescriptor(ModbusDeviceDescriptor.Ascii);
        }
        public DeviceConfigBuilder ModbusRtu()
        {
            return deviceConfigBuilder.SetDescriptor(ModbusDeviceDescriptor.Rtu);
        }
        public DeviceConfigBuilder ModbusTcp()
        {
            return deviceConfigBuilder.SetDescriptor(ModbusDeviceDescriptor.Tcp);
        }
    }
}