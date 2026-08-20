using EdgeGateway.Data;
using EdgeGateway.Ports;
using FluentResults;

namespace EdgeGateway.Devices;


public interface IDevice :  IAsyncDisposable
{
    DeviceId Id { get; }
    PortAddress PortAddress { get; }


    ValueTask<Result> ConnectAsync(CancellationToken cancellationToken = default);
    ValueTask<Result> DisconnectAsync(CancellationToken cancellationToken = default);


    ValueTask<Result<T>> ReadAsync<T>(DataDescriptor<T> descriptor, CancellationToken cancellationToken = default);
    ValueTask<Result> WriteAsync<T>(DataDescriptor<T> descriptor, T value, CancellationToken cancellationToken = default);
}