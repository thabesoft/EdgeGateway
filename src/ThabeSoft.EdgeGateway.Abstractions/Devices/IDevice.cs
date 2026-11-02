using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDevice : IDeviceLifecycle, IPollingManager, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}

public interface IOnDemandReadWrite
{
    ValueTask<T> ReadNowAsync<T>(ITag<T> tag, CancellationToken ct = default) 
        where T : notnull;

    ValueTask WriteNowAsync<T>(ITag<T> tag, T value, CancellationToken ct = default)
        where T : notnull;
}