using System.Runtime.CompilerServices;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Guard;


public static class SerialGuard
{
    extension(string param)
    {
        /// <summary>
        /// 必须是串口名称
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string PortName([CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentException("串口名称不可为空", paramName);

            var trimedPortName = param.Trim();
            if (!trimedPortName.StartsWith("com", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("串口名称格式错误, 必须以 COM 开头", paramName);

            var portNumberString = trimedPortName[3..];
            if (!int.TryParse(portNumberString, out _))
                throw new ArgumentException("串口名称格式错误, 应类似于 COM1 或 COM3", paramName);

            return trimedPortName;
        }
    }

    extension(int param)
    {
        public int StandardBaudRate([CallerArgumentExpression(nameof(param))] string? paramName = null) =>
            param.In(StandardBaudRates, $"不是标准波特率({param})", paramName);

        public int DataBits([CallerArgumentExpression(nameof(param))] string? paramName = null) =>
            param.InRange(5, 8, $"数据位({param})超出范围");
    }


    public static int[] StandardBaudRates { get; } = [1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000];
}
