using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class DeviceOptionsBuilder : IDeviceOptionsConfigurator
{
    private DeviceDescriptor? _descriptor;
    private readonly List<ISubscriptionOptions> _subscriptions = [];


    public IDeviceOptionsConfigurator SetDescriptor(DeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public IDeviceOptionsConfigurator AddSubscription<T>(Action<ISubscriptionOptionsConfigurator<T>> buildAction) where T : notnull
    {
        var builder = new SubscriptionOptionsBuilder<T>();
        buildAction?.Invoke(builder);
        var options = builder.Build();

        _subscriptions.Add(options);
        return this;
    }


    public DeviceOptions Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);

        return new()
        {
            Descriptor = _descriptor,
            Subscriptions = [.. _subscriptions]
        };
    }
}

public static class DeviceOptionsBuilderExtensions
{
    extension(IDeviceOptionsConfigurator builder)
    {
    }
}


internal sealed record class DeviceOptions
{
    public required DeviceDescriptor Descriptor { get; init; }
    public required ISubscriptionOptions[] Subscriptions { get; init; }
}