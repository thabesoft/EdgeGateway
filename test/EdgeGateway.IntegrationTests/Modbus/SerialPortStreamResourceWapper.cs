using NModbus.IO;
using System.IO.Ports;

namespace EdgeGateway.Modbus;

public sealed class SerialPortStreamResourceWapper(SerialPort serialPort) : IStreamResource
{
    public int InfiniteTimeout => -1;
    public int ReadTimeout { get => serialPort.ReadTimeout; set => serialPort.ReadTimeout = value; }
    public int WriteTimeout { get => serialPort.WriteTimeout; set => serialPort.WriteTimeout = value; }



    public int Read(byte[] buffer, int offset, int count)
    {
        return serialPort.Read(buffer, offset, count);
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        serialPort.Write(buffer, offset, count);
    }

    public void DiscardInBuffer()
    {
        serialPort.DiscardInBuffer();
    }

    public void Dispose()
    {

    }
}