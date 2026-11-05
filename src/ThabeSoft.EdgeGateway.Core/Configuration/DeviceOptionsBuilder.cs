using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class DeviceOptionsBuilder : IDeviceOptionsConfigurator
{
    private DeviceDescriptor? _descriptor;
    private readonly Dictionary<TagId, ITagOptions> _tags = [];


    public IDeviceOptionsConfigurator SetDescriptor(DeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public IDeviceOptionsConfigurator AddTag<T>(Action<ITagOptionsConfigurator<T>> buildAction) where T : notnull
    {
        var builder = new TagOptionsBuilder<T>();
        buildAction?.Invoke(builder);
        var options = builder.Build();

        _tags[options.Name] = options;
        return this;
    }


    public DeviceOptions Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);

        return new()
        {
            Descriptor = _descriptor,
            Tags = [.. _tags.Values]
        };
    }
}

public static class DeviceOptionsBuilderExtensions
{
    extension(IDeviceOptionsConfigurator builder)
    {
        public IDeviceOptionsConfigurator AddFixedIntervalTriggerTag<T>(TagDescriptor<T> tagDescriptor, TimeSpan timeSpan) where T : notnull =>
            builder.AddTag<T>(x => x.WithFixedIntervalTrigger(timeSpan).WithDescriptor(tagDescriptor));
        public IDeviceOptionsConfigurator AddTagWithFixedInterval<T>(TagDescriptor<T> tagDescriptor, double milliseconds) where T : notnull =>
           builder.AddFixedIntervalTriggerTag(tagDescriptor, TimeSpan.FromMilliseconds(milliseconds));
    }
}



internal sealed record class DeviceOptions
{
    public required DeviceDescriptor Descriptor { get; init; }
    public required IReadOnlyList<ITagOptions> Tags { get; init; }
}