using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;


public sealed class DeviceOptionsBuilder : IDeviceOptionsBuilder
{
    private IDeviceDescriptor? _descriptor;
    private readonly Dictionary<TagName, TagOptions> _tags = [];


    public IDeviceOptionsBuilder SetDescriptor(IDeviceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public IDeviceOptionsBuilder AddTag(Action<ITagOptionsBuilder> buildAction)
    {
        var builder = new TagOptionsBuilder();
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
            Descriptor = _descriptor
        };
    }
}


public sealed record class DeviceOptions
{
    public required IDeviceDescriptor Descriptor { get; init; }
}