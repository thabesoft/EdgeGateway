using FluentResults;

namespace EdgeGateway.Ports.SerialPort;


public sealed class SerialPortFactory : IPortFactory
{
    private SerialPortFactory() { }
    public static SerialPortFactory Instance { get; } = new();


    public bool CanHandle(IPortConfig config)
    {
        return config is SerialPortConfig;
    }

    public Result<IPort> Create(IPortConfig config)
    {
        if (config is not SerialPortConfig serialPortConfig)
            return new Error($"期望 SerialPortConfig，实际为 {config.GetType().Name}");

        return new SerialPortAdapter(serialPortConfig);
    }
}