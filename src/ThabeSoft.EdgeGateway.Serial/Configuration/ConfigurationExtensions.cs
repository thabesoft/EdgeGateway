using System.IO.Ports;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class ConfigurationExtensions
{
    extension(IGatewayConfigurator configurator)
    {
        public IPortConfigurator AddSerialPort(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One) =>
            configurator.AddPort(Serial.Port(portName, baudRate, parity, dataBits, stopBits));
    }
}
