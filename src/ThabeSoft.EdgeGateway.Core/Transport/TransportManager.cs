using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TransportManager(IEnumerable<ITransportFactory> factories)
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, ITransportConnection> _items = [];
    public IReadOnlyDictionary<ConnectionEndpoint, ITransportConnection> Items => _items;


    public async ValueTask<ITransportConnection> RegisterTransportAsync(ITransportDescriptor channelDescriptor)
    {
        var transportConnection = factories
            .FirstOrDefault(x => x.CanCreate(channelDescriptor))
            ?.Create(channelDescriptor)
            ?? throw new InvalidOperationException($"无法创建端口, 不支持的描述: {channelDescriptor}");

        // 添加
        var wrapper = new Entry(transportConnection, this);
        if (_items.TryAdd(channelDescriptor.Endpoint, wrapper)) return wrapper;

        // 添加失败
        await wrapper.DisposeAsync();
        throw new InvalidOperationException($"端口 {channelDescriptor.Endpoint} 已存在");
    }

    private sealed class Entry(ITransportConnection transportConnection, TransportManager transportManager) : ITransportConnection
    {
        private int _disposed;
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await transportConnection.DisposeAsync();
            transportManager._items.TryRemove(Endpoint, out _);
        }


        public ConnectionEndpoint Endpoint => transportConnection.Endpoint;
        public LifecycleState State => transportConnection.State;

        public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
            transportConnection.StartAsync(cancellationToken);
        public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
            transportConnection.StopAsync(cancellationToken);


        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            transportConnection.ReadAsync(buffer, cancellationToken);
        public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default) =>
            transportConnection.WriteAsync(data, cancellationToken);
    }
}
