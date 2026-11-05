using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : ILifecycle, IPollingManager, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}

public interface IOnDemandReadWrite
{
    ValueTask<T> ReadNowAsync<T>(TagDescriptor<T> tag, CancellationToken ct = default) 
        where T : notnull;
    ValueTask WriteNowAsync<T>(TagDescriptor<T> tag, T value, CancellationToken ct = default)
        where T : notnull;
}