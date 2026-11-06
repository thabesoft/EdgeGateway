using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TransportManager(
        IEnumerable<ITransportFactory> factories, 
        ILoggerFactory loggerFactory
    ) : IAsyncDisposable
{
    private readonly ILogger<TransportManager> _logger = loggerFactory.CreateLogger<TransportManager>();
    private readonly ConcurrentDictionary<ConnectionEndpoint, ITransportConnection> _entries = [];



    public ITransportConnection Register(ITransportDescriptor channelDescriptor)
    {
        var transportConnection = factories
            .FirstOrDefault(x => x.CanCreate(channelDescriptor))
            ?.Create(channelDescriptor)
            ?? throw new InvalidOperationException($"无法创建端口, 不支持的描述: {channelDescriptor}");

        // 添加
        var wrapper = new Entry(transportConnection, this, _logger);
        if (_entries.TryAdd(channelDescriptor.Endpoint, wrapper)) return wrapper;

        // 添加失败
        wrapper.Dispose();
        throw new InvalidOperationException($"端口 {channelDescriptor.Endpoint} 已存在");
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var i in _entries.Values)
            await i.DisposeAsync();

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("传输管理器已释放");
    }



    private sealed class Entry(ITransportConnection transportConnection, TransportManager transportManager, ILogger logger) : ITransportConnection, IDisposable
    {
        private int _disposed;
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await transportConnection.DisposeAsync();
            transportManager._entries.TryRemove(Endpoint, out _);

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("传输器 [{Endpoint}] 已释放", Endpoint);
        }

        public async void Dispose()
        {
            await DisposeAsync();
        }



        public ConnectionEndpoint Endpoint => transportConnection.Endpoint;
        public LifecycleState State => transportConnection.State;


        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("传输器 [{Endpoint}] 启动", Endpoint);

            return transportConnection.StartAsync(cancellationToken);
        }
        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("传输器 [{Endpoint}] 停止", Endpoint);

            return transportConnection.StopAsync(cancellationToken);
        }


        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("传输器读取, Length:{buffer}", buffer.Length);

            return transportConnection.ReadAsync(buffer, cancellationToken);
        }
        public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("传输器写入, Length:{buffer}", data.Length);

            return transportConnection.WriteAsync(data, cancellationToken);
        }
    }
}
