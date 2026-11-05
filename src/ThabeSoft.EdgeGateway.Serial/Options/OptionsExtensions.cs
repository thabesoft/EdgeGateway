using System.IO.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class OptionsExtensions
{
    extension(IGatewayOptionsConfigurator builder)
    {
        public IGatewayOptionsConfigurator AddSerialPort(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Action<IPortOptionsConfigurator>? configure = null) =>
            builder.AddPort(x => { configure?.Invoke(x); x.UseSerial(portName, baudRate, parity, dataBits, stopBits); });

    }

    extension(IPortOptionsConfigurator builder)
    {
        public IPortOptionsConfigurator UseSerial(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One) =>
            builder.WithTransport(SerialTransportDescriptor.Create(portName, baudRate, parity, dataBits, stopBits));
    }
}
