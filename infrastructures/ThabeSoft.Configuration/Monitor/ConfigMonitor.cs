using ThabeSoft.PubSub;

namespace ThabeSoft.Configuration.Monitor;


internal sealed class ConfigMonitor<T> : IConfigMonitor<T>, IDisposable
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IConfigSerializer _configSerializer;
    private readonly IEventSubscriber _eventSubscriber;
    private readonly ConfigFileWatcher _fileWatcher;


    public ConfigMonitor(
            IEventPublisher eventPublisher,
            IEventSubscriber eventSubscriber,
            IConfigSerializer configSerializer,
            ConfigFileWatcher fileWatcher
        )
    {
        _fileWatcher = fileWatcher;
        fileWatcher.Changed += OnFileChanged;

        _eventPublisher = eventPublisher;
        _configSerializer = configSerializer;
        _eventSubscriber = eventSubscriber;
        _fileWatcher = fileWatcher;
    }

    public T? CurrentValue { get; private set; }

    public IDisposable OnChange(Func<T, CancellationToken, ValueTask> action)
    {
        return _eventSubscriber.Subscribe<ConfigChangeMessage>((msg, ct) => action(msg.Config, ct));
    }
    public void Dispose()
    {
        _fileWatcher.Changed -= OnFileChanged;
    }


    private async Task OnFileChanged(CancellationToken cancellationToken)
    {
        CurrentValue = await _configSerializer.DeserializeFileAsync<T>(_fileWatcher.FilePath, cancellationToken);
        _ = _eventPublisher.PublishAsync(new ConfigChangeMessage(CurrentValue), cancellationToken);
    }

    private record class ConfigChangeMessage(T Config) : IMessage;
}