using System.IO.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Options;


public static class OptionsExtensions
{
    extension(IGatewayOptionsBuilder builder)
    {
        public IGatewayOptionsBuilder AddSerialPort(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Action<IPortOptionsBuilder>? configure = null) =>
            builder.AddPort(x => { configure?.Invoke(x); x.UseSerial(portName, baudRate, parity, dataBits, stopBits); });

    }

    extension(IPortOptionsBuilder builder)
    {
        public IPortOptionsBuilder UseSerial(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One) =>
            builder.WithTransport(SerialTransportDescriptor.Create(portName, baudRate, parity, dataBits, stopBits));
    }
}
