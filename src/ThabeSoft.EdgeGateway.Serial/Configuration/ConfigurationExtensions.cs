using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


public static class ConfigurationExtensions
{
    extension(IGatewayConfigurator configurator)
    {
        public IPortConfigurator AddSerialPort(string portName,
            int baudRate = 9600,
            SerialParity parity = SerialParity.None,
            int dataBits = 8,
            SerialStopBits stopBits = SerialStopBits.One
        ) => configurator.AddPort(Serial.Port(portName, baudRate, parity, dataBits, stopBits));
    }
}
