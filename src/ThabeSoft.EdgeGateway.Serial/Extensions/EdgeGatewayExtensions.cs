using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Configs;
using ThabeSoft.EdgeGateway.Ports;
using System.IO.Ports;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class EdgeGatewayExtensions
{
    extension(PortConfigBuilder portConfigBuilder)
    {
        /// <summary>
        /// 使用串口通道
        /// </summary>
        public PortConfigBuilder Serial(Action<SerialChannelDescriptorBuilder> buildAction)
        {
            var builder = SerialChannelDescriptor.Builder();
            buildAction.Invoke(builder);

            return portConfigBuilder.SetDescriptor(builder.Build());
        }

        public PortConfigBuilder Serial(SerialChannelDescriptor descriptor)
        {
            return portConfigBuilder.SetDescriptor(descriptor);
        }

        public PortConfigBuilder Serial(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            return portConfigBuilder.Serial(x => x
                .PortName(portName)
                .BaudRate(baudRate)
                .Parity(parity)
                .DataBits(dataBits)
                .StopBits(stopBits));
        }
    }

    extension(IPortManager portManager)
    {
        public IPort AddSerialPort(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            return portManager.AddSerialPort(x => x
                .PortName(portName)
                .BaudRate(baudRate)
                .Parity(parity)
                .DataBits(dataBits)
                .StopBits(stopBits));
        }

        public IPort AddSerialPort(Action<SerialChannelDescriptorBuilder> buildAction)
        {
            var builder = new SerialChannelDescriptorBuilder();
            buildAction.Invoke(builder);

            return portManager.AddPort(builder.Build());
        }
    }
}