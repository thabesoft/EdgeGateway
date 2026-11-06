using System.IO.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public static class Serial
{
    public static SerialTransportDescriptor Port(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        => SerialTransportDescriptor.Create(portName, baudRate, parity, dataBits, stopBits);
}
