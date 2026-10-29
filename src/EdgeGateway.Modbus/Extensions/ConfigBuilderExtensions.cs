using EdgeGateway.Configs;
using EdgeGateway.Modbus;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class ConfigBuilderExtensions
{
    extension(PortConfigBuilder portConfigBuilder)
    {
        public PortConfigBuilder AddModbusDevice(ModbusFrameFormat frameFormat)
        {
            var builder = DeviceConfig.Builder()
                .Modbus(frameFormat);

            return portConfigBuilder.AddDevice(builder.Build());
        }

        public PortConfigBuilder AddModbusRtuDevice() =>
            portConfigBuilder.AddModbusDevice(ModbusFrameFormat.Rtu);

        public PortConfigBuilder AddModbusAsciiDevice() =>
            portConfigBuilder.AddModbusDevice(ModbusFrameFormat.Ascii);

        public PortConfigBuilder AddModbusIpDevice() =>
            portConfigBuilder.AddModbusDevice(ModbusFrameFormat.Ip);
    }

    extension(DeviceConfigBuilder deviceConfigBuilder)
    {
        public DeviceConfigBuilder Modbus(ModbusFrameFormat frameFormat)
        {
            if (frameFormat == ModbusFrameFormat.Rtu)
                return deviceConfigBuilder.ModbusRtu();

            if (frameFormat == ModbusFrameFormat.Ascii)
                return deviceConfigBuilder.ModbusAscii();

            if (frameFormat == ModbusFrameFormat.Ip)
                return deviceConfigBuilder.ModbusTcp();


            throw new ArgumentOutOfRangeException(nameof(frameFormat));
        }


        public DeviceConfigBuilder ModbusAscii() =>
            deviceConfigBuilder.Serial(ModbusProtocolDescriptor.Ascii);
        public DeviceConfigBuilder ModbusRtu() =>
            deviceConfigBuilder.Serial(ModbusProtocolDescriptor.Rtu);
        public DeviceConfigBuilder ModbusTcp() =>
            deviceConfigBuilder.Ethernet(ModbusProtocolDescriptor.Ip);
    }
}