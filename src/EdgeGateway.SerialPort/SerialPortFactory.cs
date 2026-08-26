using EdgeGateway.Ports;
using FluentResults;

namespace EdgeGateway.SerialPort;


internal sealed class SerialPortFactory : IPortFactory
{
    public bool CanHandle(IPortOptions options)
    {
        return options is SerialPortOptions;
    }

    public Result<IPort> Create(IPortOptions options)
    {
        if (options is not SerialPortOptions serialPortConfig)
            return new Error($"期望 SerialPortConfig，实际为 {options.GetType().Name}");

        return new SerialPortAdapter(serialPortConfig);
    }
}