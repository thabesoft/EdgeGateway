using System.IO.Ports;

namespace EdgeGateway.Ports.SerialPort;


/// <summary>
/// 串口配置
/// </summary>
public sealed record class SerialPortConfig : IPortConfig
{
    public PortAddress Address { get; private set; } = default!;

    public string PortName { get; private set; } = string.Empty;
    public int BaudRate { get; private set; } = 9600;
    public Parity Parity { get; private set; } = Parity.None;
    public int DataBits { get; private set; } = 8;
    public StopBits StopBits { get; private set; } = StopBits.One;


    private SerialPortConfig() { }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 为空或仅包含空白字符时抛出</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> 为 null 时抛出</exception>
    public static SerialPortConfig Create(string name)
    {
        var config = new SerialPortConfig();

        config.SetName(name)
            .SetBaudRate(9600)
            .SetParity(Parity.None)
            .SetDataBits(8)
            .SetStopBits(StopBits.One);

        config.Address = PortAddress.Create(PortType.SerialPort, config.PortName);

        return config;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortConfig SetName(string name)
    {
        if(name is null) throw new ArgumentNullException(nameof(name), "串口名称不能为空");

        var trimmed = name.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ArgumentException(nameof(name), "串口名称不能为空白");

        PortName = trimmed;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public SerialPortConfig SetBaudRate(int baudRate)
    {
        if (!IsValidBaudRate(baudRate)) throw new ArgumentOutOfRangeException(nameof(baudRate), $"波特率(BaudRate) {baudRate} 无效，请使用标准波特率");

        BaudRate = baudRate;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortConfig SetParity(Parity parity)
    {
        if (!Enum.IsDefined(parity)) throw new ArgumentException($"校验值(Parity) {parity} 不存在", nameof(parity));
        if (!IsValidParityAndDataBits(parity, DataBits)) throw new ArgumentException("当数据(DataBits)位为 5 时，校验位（Parity）必须为 None");

        Parity = parity;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortConfig SetDataBits(int dataBits)
    {
        if (dataBits < 5 || dataBits > 8) throw new ArgumentOutOfRangeException(nameof(dataBits), $"数据位(DataBits) {dataBits} 无效, 必须在5到8之间");
        if (!IsValidParityAndDataBits(Parity, dataBits)) throw new ArgumentException("当校验位(Parity)为 None 时，数据位(DataBits)必须大于 5");

        DataBits = dataBits;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public SerialPortConfig SetStopBits(StopBits stopBits)
    {
        if (!Enum.IsDefined(stopBits)) throw new ArgumentException($"停止位(StopBits) {stopBits} 值不存在", nameof(stopBits));
        if (stopBits == StopBits.None) throw new ArgumentException("停止位(StopBits)不能为None", nameof(stopBits));

        StopBits = stopBits;
        return this;
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
