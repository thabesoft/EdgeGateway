using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TransportManager(IEnumerable<ITransportFactory> factories, ILoggerFactory loggerFactory)
{
    private readonly ILogger<TransportManager> _logger = loggerFactory.CreateLogger<TransportManager>();
    private readonly ConcurrentDictionary<ConnectionEndpoint, ITransportConnection> _items = [];


    public ITransportConnection Register(ITransportDescriptor channelDescriptor)
    {
        var transportConnection = factories
            .FirstOrDefault(x => x.CanCreate(channelDescriptor))
            ?.Create(channelDescriptor)
            ?? throw new InvalidOperationException($"无法创建端口, 不支持的描述: {channelDescriptor}");

        // 添加
        var wrapper = new Entry(transportConnection, this, _logger);
        if (_items.TryAdd(channelDescriptor.Endpoint, wrapper)) return wrapper;

        // 添加失败
        wrapper.Dispose();
        throw new InvalidOperationException($"端口 {channelDescriptor.Endpoint} 已存在");
    }

    private sealed class Entry(ITransportConnection transportConnection, TransportManager transportManager, ILogger logger) : ITransportConnection, IDisposable
    {
        private int _disposed;
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await transportConnection.DisposeAsync();
            transportManager._items.TryRemove(Endpoint, out _);
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
                logger.LogDebug("传输器启动 [{Endpoint}]", Endpoint);

            return transportConnection.StartAsync(cancellationToken);
        }
        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("传输器停止 [{Endpoint}]", Endpoint);

            return transportConnection.StopAsync(cancellationToken);
        }


        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("传输器读取, Length:{buffer}", buffer.Length);

            return transportConnection.ReadAsync(buffer, cancellationToken);
        }
        public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("传输器写入, Length:{buffer}", data.Length);

            return transportConnection.WriteAsync(data, cancellationToken);
        }
    }
}
