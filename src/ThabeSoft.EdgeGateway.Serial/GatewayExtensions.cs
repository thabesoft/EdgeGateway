using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;

public static class GatewayExtensions
{
    extension(IGateway gateway)
    {
        public ValueTask<IPort> AddSerialPort(string portName,
            int baudRate = 9600,
            SerialParity parity = SerialParity.None,
            int dataBits = 8,
            SerialStopBits stopBits = SerialStopBits.One,
            CancellationToken cancellationToken = default)
        => gateway.AddPortAsync(Serial.Port(portName, baudRate, parity, dataBits, stopBits), cancellationToken);
    }
}
