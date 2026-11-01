using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.IO;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DefaultDeviceFactory(
        ILoggerFactory loggerFactory,
        IEnumerable<ITagAccessorFactory> tagAccessorFactories
    ) : IDeviceFactory
{
    private readonly ITagAccessorFactory[] _tagAccessorFactories = [.. tagAccessorFactories];


    public bool CanCreate(IDeviceDescriptor descriptor)
    {
        return true;
    }

    public IDevice Create(IDeviceDescriptor descriptor, IByteStream byteStream)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(byteStream);

        var tagReaderFactory = _tagAccessorFactories.FirstOrDefault(x => x.CanCreate(descriptor.Protocol));
        ArgumentNullException.ThrowIfNull(tagReaderFactory);


        var tagAccessor = tagReaderFactory.Create(descriptor.Protocol, byteStream);
        var logger = loggerFactory.CreateLogger<Device>();

        return new Device(tagAccessor, logger)
        {
            Id = descriptor.Id,
            ProtocolType = descriptor.Protocol.Type
        };
    }
}


file sealed class Device(
        ITagAccessor tagAccessor,
        ILogger<Device> logger
    ) : ConcurrentLifecycleBase(logger), IDevice, IOnDemandReadWrite
{
    private readonly PollingScheduler _pollingScheduler = new(tagAccessor);


    public required DeviceId Id { get; init; }
    public required ProtocolType ProtocolType { get; init; }
    public bool IsEnabled => IsRunning;
    public IReadOnlyDictionary<TagName, ITagMonitor> Monitors => _pollingScheduler.Monitors;


    public ValueTask EnableAsync(CancellationToken cancellationToken = default) =>
        StartAsync(cancellationToken);
    public ValueTask DisableAsync(CancellationToken cancellationToken = default) =>
        StopAsync(cancellationToken);


    public ValueTask<T> ReadNowAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default) where T : notnull =>
        tagAccessor.ReadAsync(tag, cancellationToken);
    public ValueTask WriteNowAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default) where T : notnull =>
        tagAccessor.WriteAsync(tag, value, cancellationToken);


    public ITagMonitor<T> Register<T>(TagName tagName, ITag<T> tag, ITrigger pollingTrigger) where T : notnull =>
        _pollingScheduler.Register(tagName, tag, pollingTrigger);
    public void Unregister(TagName tagName) =>
        _pollingScheduler.Unregister(tagName);


    protected override ValueTask OnStartAsync(CancellationToken cancellationToken) =>
        _pollingScheduler.StartAsync(cancellationToken);
    protected override ValueTask OnStopAsync(CancellationToken cancellationToken) =>
        _pollingScheduler.StopAsync(cancellationToken);
    protected override ValueTask OnDisposeAsync() =>
        _pollingScheduler.DisposeAsync();
}