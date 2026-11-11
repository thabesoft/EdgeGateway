namespace ThabeSoft.EdgeGateway.Serial;


public static class SerialTransport
{
    public static SerialTransportDescriptor Create(string portName,
        int baudRate = 9600,
        SerialParity parity = SerialParity.None,
        int dataBits = 8,
        SerialStopBits stopBits = SerialStopBits.One
    ) => SerialTransportDescriptor.Create(portName, baudRate, parity, dataBits, stopBits);
}
