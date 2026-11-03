using System.IO.Ports;
using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.Options;


public static class OptionsExtensions
{
    extension(IGatewayOptionsBuilder builder)
    {
        public IGatewayOptionsBuilder AddSerialPort(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Action<IPortOptionsBuilder>? portBuildAction = null) =>
            builder.AddPort(x => { portBuildAction?.Invoke(x); x.UseSerial(portName, baudRate, parity, dataBits, stopBits); });
    }

    extension(IPortOptionsBuilder builder)
    {
        public IPortOptionsBuilder UseSerial(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One) =>
            builder.SetDescriptor(SerialChannelDescriptor.Create(portName, baudRate, parity, dataBits, stopBits));
    }
}
