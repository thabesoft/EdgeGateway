using System.Diagnostics.CodeAnalysis;

namespace ThabeSoft.EdgeGateway.Transport;


public sealed record class SerialTransportDescriptor : ITransportDescriptor
{
    public required ConnectionEndpoint Endpoint { get; init; }
    public required string PortName { get; init; }
    public required int BaudRate { get; init; }
    public required int DataBits { get; init; }
    public required SerialParity Parity { get; init; }
    public required SerialStopBits StopBits { get; init; }


    private SerialTransportDescriptor() { }
    // COM2(9600, 8N5)
    public override string ToString()
    {
        var parityChar = Parity switch
        {
            SerialParity.None => 'N',
            SerialParity.Even => 'E',
            SerialParity.Odd => 'O',
            SerialParity.Mark => 'M',
            SerialParity.Space => 'S',
            _ => '?'
        };

        var stopBitsChar = StopBits switch
        {
            SerialStopBits.One => '1',
            SerialStopBits.Two => '2',
            SerialStopBits.OnePointFive => '5',
            _ => '?'
        };

        return $"{PortName}({BaudRate}, {DataBits}{parityChar}{stopBitsChar})";
    }


    public static SerialTransportDescriptor Create(string portName, int baudRate, SerialParity parity, int dataBits, SerialStopBits stopBits)
    {
        if (!ValidatePortName(portName, out var portNameErrorMessage))
            throw new ArgumentException(portNameErrorMessage, nameof(portName));

        if (!ValidateBaudRate(baudRate, out var baudRateErrorMessage))
            throw new ArgumentOutOfRangeException(nameof(baudRate), baudRate, baudRateErrorMessage);

        if (!ValidateParity(parity, out var parityErrorMessage))
            throw new ArgumentOutOfRangeException(nameof(parity), parity, parityErrorMessage);

        if (!ValidateDataBits(dataBits, out var dataBitsErrorMessage))
            throw new ArgumentOutOfRangeException(nameof(dataBits), dataBits, dataBitsErrorMessage);

        if (!ValidateStopBits(stopBits, out var stopBitsErrorMessage))
            throw new ArgumentOutOfRangeException(nameof(stopBits), stopBits, stopBitsErrorMessage);


        return new()
        {
            Endpoint = ConnectionEndpoint.Serial(portName),
            PortName = portName,
            BaudRate = baudRate,
            Parity = parity,
            DataBits = dataBits,
            StopBits = stopBits
        };
    }



    public static bool ValidatePortName(ReadOnlySpan<char> portName, [NotNullWhen(false)] out string? errorMessage)
    {
        if (portName.IsWhiteSpace())
        {
            errorMessage = $"串口名称不可为空";
            return false;
        }

        var trimedPortName = portName.Trim();
        if (!trimedPortName.StartsWith("com", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = $"串口名称格式错误, 必须以 COM 开头";
            return false;
        }
        var portNumberString = trimedPortName[3..];
        if (!int.TryParse(portNumberString, out _))
        {
            errorMessage = $"串口名称格式错误, 应类似于 COM1 或 COM3";
            return false;
        }

        errorMessage = null;
        return true;
    }
    public static int[] StandardBaudRates { get; } = [1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000];
    public static bool ValidateBaudRate(int? baudRate, [NotNullWhen(false)] out string? errorMessage)
    {
        if (baudRate is null)
        {
            errorMessage = $"波特率不可为空";
            return false;
        }

        if (baudRate <= 0)
        {
            errorMessage = $"波特率必须> 0";
            return false;
        }

        if (!StandardBaudRates.Contains(baudRate.Value))
        {
            errorMessage = "请输入标准的波特率（如 9600, 115200）";
            return false;
        }

        errorMessage = null;
        return true;
    }
    public static bool ValidateParity(SerialParity parity, [NotNullWhen(false)] out string? errorMessage)
    {
        if (parity is not (SerialParity.None or SerialParity.Odd or SerialParity.Even or SerialParity.Mark or SerialParity.Space))
        {
            errorMessage = "无效的校验位，仅支持：None, Odd, Even, Mark, Space";
            return false;
        }

        errorMessage = null;
        return true;
    }
    public static bool ValidateDataBits(int dataBits, [NotNullWhen(false)] out string? errorMessage)
    {
        if (dataBits is < 5 or > 8)
        {
            errorMessage = "无效的数据位，范围只能在 [5~8] 之间";
            return false;
        }

        errorMessage = null;
        return true;
    }
    public static bool ValidateStopBits(SerialStopBits stopBits, [NotNullWhen(false)] out string? errorMessage)
    {
        if (stopBits is not (SerialStopBits.One or SerialStopBits.OnePointFive or SerialStopBits.Two))
        {
            errorMessage = "无效的停止位，仅支持：One, OnePointFive, Tow";
            return false;
        }

        errorMessage = null;
        return true;
    }
}
