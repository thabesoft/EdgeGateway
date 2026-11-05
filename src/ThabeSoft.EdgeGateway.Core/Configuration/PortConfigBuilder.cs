using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class PortConfigBuilder : IPortOptionsConfigurator
{
    private ITransportDescriptor? _transport;
    private readonly List<DeviceOptions> _devices = [];


    public IPortOptionsConfigurator WithTransport(ITransportDescriptor transportDescriptor)
    {
        ArgumentNullException.ThrowIfNull(transportDescriptor);

        _transport = transportDescriptor;
        return this;
    }
    public IPortOptionsConfigurator AddDevice(Action<IDeviceOptionsConfigurator> buildAction)
    {
        var builder = new DeviceOptionsBuilder();
        buildAction.Invoke(builder);
        var deviceOptions = builder.Build();

        if (_devices.Contains(deviceOptions))
            return this;

        _devices.Add(deviceOptions);
        return this;
    }


    public PortOptions Build()
    {
        if (_transport is null)
            throw new ArgumentNullException(nameof(_transport), "未初始化端口信息");

        return new PortOptions()
        {
            Transport = _transport,
            Devices = _devices
        };
    }
}


internal sealed record class PortOptions
{
    public required ITransportDescriptor Transport { get; init; }
    public required IReadOnlyList<DeviceOptions> Devices { get; init; }
}
