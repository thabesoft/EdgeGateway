using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway;


internal sealed class Device : ConcurrentLifecycleBase, IDevice, IOnDemandReadWrite
{
    private ITagAccessor _tagAccessor;
    private readonly PollingScheduler _pollingScheduler;
    private readonly DeviceDisposedHandler _disposedCallbackHandler;

    public Device(
            IDeviceDescriptor descriptor,
            TagAccessorFactoryHandler tagAccessorFactoryHandler,
            DeviceDisposedHandler disposedCallbackHandler,
            ILogger<Device> logger
        ) : base(logger)
    {
        Id = descriptor.DeviceId;
        ProtocolType = descriptor.Protocol.Type;

        _tagAccessor = tagAccessorFactoryHandler();
        _pollingScheduler = new PollingScheduler(_tagAccessor);
        _disposedCallbackHandler = disposedCallbackHandler;
    }

    public DeviceId Id { get; }
    public ProtocolType ProtocolType { get; }
    public bool IsEnabled => IsRunning;
    public IReadOnlyDictionary<TagName, ITagMonitor> TagMonitors => _pollingScheduler.TagMonitors;


    public ValueTask EnableAsync(CancellationToken cancellationToken = default) =>
        StartAsync(cancellationToken);
    public ValueTask DisableAsync(CancellationToken cancellationToken = default) =>
        StopAsync(cancellationToken);


    public ValueTask<T> ReadNowAsync<T>(ITagDescriptor<T> tag, CancellationToken cancellationToken = default) where T : notnull =>
        _tagAccessor.ReadAsync(tag, cancellationToken);
    public ValueTask WriteNowAsync<T>(ITagDescriptor<T> tag, T value, CancellationToken cancellationToken = default) where T : notnull =>
        _tagAccessor.WriteAsync(tag, value, cancellationToken);


    public ITagMonitor<T> Register<T>(ITagDescriptor<T> tag, ITrigger trigger) where T : notnull =>
        _pollingScheduler.Register(tag, trigger);
    public void Unregister(TagName tagName) =>
        _pollingScheduler.Unregister(tagName);


    protected override ValueTask OnStartAsync(CancellationToken cancellationToken) =>
        _pollingScheduler.StartAsync(cancellationToken);
    protected override ValueTask OnStopAsync(CancellationToken cancellationToken) =>
        _pollingScheduler.StopAsync(cancellationToken);
    protected override async ValueTask OnDisposeAsync()
    {
        await _pollingScheduler.DisposeAsync();
        await _disposedCallbackHandler.Invoke(Id);
    }
}

internal delegate ITagAccessor TagAccessorFactoryHandler();
internal delegate ValueTask DeviceDisposedHandler(DeviceId id);