using System.Diagnostics.CodeAnalysis;
using ThabeSoft.EdgeGateway.Guard;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Transports;


public sealed record class SerialTransportDescriptor : TransportDescriptor
{
    public required string PortName { get; init => field = value.PortName(); }
    public int BaudRate { get; init => field = value.StandardBaudRate(); } = 9600;
    public int DataBits { get; init => field = value.DataBits(); } = 8;
    public SerialParity Parity { get; init => field = value.Defined($"未知的校验值({value})"); } = SerialParity.None;
    public SerialStopBits StopBits { get; init => field = value.Defined($"未知的停止位({value})"); } = SerialStopBits.One;

    public override ConnectionEndpoint Endpoint => ConnectionEndpoint.Serial(PortName);




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
        if (!int.TryParse(portNumberString, null, out _))
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
