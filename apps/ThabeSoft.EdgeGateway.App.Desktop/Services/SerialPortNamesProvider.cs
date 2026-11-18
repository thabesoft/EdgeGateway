using System.IO.Ports;
using ThabeSoft.EdgeGateway.App.Services;

namespace ThabeSoft.EdgeGateway.App.Desktop.Services;

internal class SerialPortNamesProvider : ISerialPortNamesProvider
{
    public string[] GetPortNames()
    {
        return SerialPort.GetPortNames();
    }
}
