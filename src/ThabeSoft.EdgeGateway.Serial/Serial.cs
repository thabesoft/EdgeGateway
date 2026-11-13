using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public static class Serial
{
    public static SerialTransportDescriptor Create(string portName,
        int baudRate = 9600,
        SerialParity parity = SerialParity.None,
        int dataBits = 8,
        SerialStopBits stopBits = SerialStopBits.One
    ) => new(portName, baudRate, parity, dataBits, stopBits);
}
