using System.IO.Ports;

namespace EdgeGateway.Serial;


public class ScopeSerialPort(SerialPortDescriptor descriptor) : IAsyncDisposable
{
    protected readonly SerialPort SerialPort = new(descriptor.PortName, descriptor.BaudRate, descriptor.Parity, descriptor.DataBits, descriptor.StopBits);

    public virtual ValueTask OpenAsync()
    {
        try
        {
            SerialPort.Open();
        }
        catch
        {

        }
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask CloseAsync()
    {
        SerialPort.Close();
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        SerialPort.Dispose();
        return ValueTask.CompletedTask;
    }
}

public sealed record SerialPortDescriptor(string PortName, int BaudRate = 9600, Parity Parity = Parity.None, int DataBits = 8, StopBits StopBits = StopBits.One);