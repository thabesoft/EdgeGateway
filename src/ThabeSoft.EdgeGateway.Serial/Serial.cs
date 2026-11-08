using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public static class Serial
{
    public static SerialTransportDescriptor Port(string portName,
        int baudRate = 9600,
        SerialParity parity = SerialParity.None,
        int dataBits = 8,
        SerialStopBits stopBits = SerialStopBits.One
    ) => SerialTransportDescriptor.Create(portName, baudRate, parity, dataBits, stopBits);
}
