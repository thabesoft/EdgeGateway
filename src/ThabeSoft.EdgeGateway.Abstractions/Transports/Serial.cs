using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway;


public static class Serial
{
    public static SerialTransportDescriptor Port(string portName,
            int baudRate = 9600,
            SerialParity parity = SerialParity.None,
            int dataBits = 8,
            SerialStopBits stopBits = SerialStopBits.One,
            RetryDescriptor? retry = null
        )
    {
        return new()
        {
            PortName = portName,
            BaudRate = baudRate,
            DataBits = dataBits,
            Parity = parity,
            StopBits = stopBits,
            Retry = retry ?? Retry.Default
        };
    }
}
