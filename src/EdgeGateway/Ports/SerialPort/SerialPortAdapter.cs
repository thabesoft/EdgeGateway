using FluentResults;

namespace EdgeGateway.Ports.SerialPort;


/// <summary>
/// 串口设备驱动适配器
/// </summary>
public sealed class SerialPortAdapter(SerialPortConfig config) : StreamPort(config.Address), IBufferDiscardable
{
    private readonly System.IO.Ports.SerialPort _serialPort = new(config.PortName, config.BaudRate, config.Parity, config.DataBits, config.StopBits);


    protected override Stream Stream => _serialPort.BaseStream;

    protected override ValueTask<Result> OpenAsync(CancellationToken cancellationToken = default)
    {
        var result = Result.Try(_serialPort.Open);
        return ValueTask.FromResult(result);
    }
    protected override ValueTask<Result> CloseAsync(CancellationToken cancellationToken = default)
    {
        var result = Result.Try(_serialPort.Close);
        return ValueTask.FromResult(result);
    }
    public override ValueTask DisposeAsync()
    {
        _serialPort.Dispose();
        return default;
    }

    public void DiscardInBuffer()
    {
        _serialPort.DiscardInBuffer();
    }
}