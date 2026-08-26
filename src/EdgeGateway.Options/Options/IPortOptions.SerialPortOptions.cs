using EdgeGateway.Ports;
using System.IO.Ports;

namespace EdgeGateway.Options;


/// <summary>
/// 串口配置
/// </summary>
public sealed record class SerialPortOptions : IPortOptions
{
    public required PortAddress Address { get; init; }
    public required string PortName { get; init; }

    public int BaudRate { get; init; } = 9600;
    public Parity Parity { get; init; } = Parity.None;
    public int DataBits { get; init; } = 8;
    public StopBits StopBits { get; init; } = StopBits.One;


    internal SerialPortOptions() { }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"><paramref name="portName"/> 为空或仅包含空白字符时抛出</exception>
    /// <exception cref="ArgumentException"><paramref name="portName"/> 为 null 时抛出</exception>
    public static SerialPortOptionsBuilder Builder(string portName)
    {
        if (portName is null) throw new ArgumentNullException(nameof(portName), "端口名不可为空");
        if (string.IsNullOrWhiteSpace(portName)) throw new ArgumentNullException(nameof(portName), "端口名不可为空白");


        return new SerialPortOptionsBuilder().SetName(portName);
    }
}


public sealed class SerialPortOptionsBuilder
{
    private string? _portName;
    private int _baudRate = 9600;
    private Parity _parity = Parity.None;
    private int _dataBits = 8;
    private StopBits _stopBits = StopBits.One;


    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortOptionsBuilder SetName(string name)
    {
        if (name is null) throw new ArgumentNullException(nameof(name), "串口名称不能为空");

        var trimmed = name.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ArgumentException(nameof(name), "串口名称不能为空白");

        _portName = trimmed;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public SerialPortOptionsBuilder SetBaudRate(int baudRate)
    {
        if (!IsValidBaudRate(baudRate)) throw new ArgumentOutOfRangeException(nameof(baudRate), $"波特率(BaudRate) {baudRate} 无效，请使用标准波特率");

        _baudRate = baudRate;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortOptionsBuilder SetParity(Parity parity)
    {
        if (!Enum.IsDefined(parity)) throw new ArgumentException($"校验值(Parity) {parity} 不存在", nameof(parity));
        if (!IsValidParityAndDataBits(parity, _dataBits)) throw new ArgumentException("当数据(DataBits)位为 5 时，校验位（Parity）必须为 None");

        _parity = parity;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortOptionsBuilder SetDataBits(int dataBits)
    {
        if (dataBits < 5 || dataBits > 8) throw new ArgumentOutOfRangeException(nameof(dataBits), $"数据位(DataBits) {dataBits} 无效, 必须在5到8之间");
        if (!IsValidParityAndDataBits(_parity, dataBits)) throw new ArgumentException("当校验位(Parity)为 None 时，数据位(DataBits)必须大于 5");

        _dataBits = dataBits;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortOptionsBuilder SetStopBits(StopBits stopBits)
    {
        if (!Enum.IsDefined(stopBits)) throw new ArgumentException($"停止位(StopBits) {stopBits} 值不存在", nameof(stopBits));
        if (stopBits == StopBits.None) throw new ArgumentException("停止位(StopBits)不能为None", nameof(stopBits));

        _stopBits = stopBits;
        return this;
    }


    public SerialPortOptions Build()
    {
        if (_portName is null) throw new ArgumentNullException(nameof(_portName), "端口名不可为空");
        if (string.IsNullOrWhiteSpace(_portName)) throw new ArgumentNullException(nameof(_portName), "端口名不可为空白");

        return new SerialPortOptions()
        {
            Address = PortAddress.Create(PortType.SerialPort, _portName),
            PortName = _portName,
            BaudRate = _baudRate,
            DataBits = _dataBits,
            Parity = _parity,
            StopBits = _stopBits
        };
    }


    private static bool IsValidParityAndDataBits(Parity parity, int dataBits)
    {
        return !(parity == Parity.None && dataBits == 5);
    }

    private static bool IsValidBaudRate(int baudRate)
    {
        return baudRate switch
        {
            110 or 300 or 600 or 1200 or 2400 or 4800 or 9600 or 14400 or 19200 or 38400 or 57600 or 115200 or 128000 or 230400 or 256000 or 460800 or 921600 => true,
            _ => false,
        };
    }
}