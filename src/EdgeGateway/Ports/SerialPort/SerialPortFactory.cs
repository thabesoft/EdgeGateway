using EdgeGateway.Options;
using FluentResults;

namespace EdgeGateway.Ports.SerialPort;


public sealed class SerialPortFactory : IPortFactory
{
    private SerialPortFactory() { }
    public static SerialPortFactory Instance { get; } = new();


    public bool CanHandle(IPortOptions config)
    {
        return config is SerialPortOptions;
    }

    public Result<IPort> Create(IPortOptions config)
    {
        if (config is not SerialPortOptions serialPortConfig)
            return new Error($"期望 SerialPortConfig，实际为 {config.GetType().Name}");

        return new SerialPortAdapter(serialPortConfig);
    }
}