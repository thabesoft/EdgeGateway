using EdgeGateway.Byte;
using EdgeGateway.Polling;
using EdgeGateway.Tag;

namespace EdgeGateway.Device;


internal sealed class DefaultDeviceFactory(
        IEnumerable<ITagReaderFactory> tagReaderFactories,
        IEnumerable<ITagWriterFactory> tagWriterFactories
    ) : IDeviceFactory
{
    private readonly ITagReaderFactory[] _tagReaderFactories = [.. tagReaderFactories];
    private readonly ITagWriterFactory[] _tagWriterFactories = [.. tagWriterFactories];


    public bool CanCreate(IDeviceDescriptor descriptor)
    {
        return true;
    }

    public IDevice Create(IDeviceDescriptor descriptor, IByteStream byteStream)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(byteStream);


        var tagReaderFactory = _tagReaderFactories.FirstOrDefault(x => x.CanCreate(descriptor.Protocol));
        ArgumentNullException.ThrowIfNull(tagReaderFactory);

        var tagReader = tagReaderFactory.Create(descriptor.Protocol, byteStream);
        var pollingScheduler = new PollingScheduler(tagReader);

        return new DefaultDevice(pollingScheduler)
        {
            Id = DeviceId.Create(),
            ProtocolType = descriptor.Protocol.Type
        };
    }
}
