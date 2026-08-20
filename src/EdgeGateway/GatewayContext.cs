using EdgeGateway.Devices;
using EdgeGateway.Ports;
using EdgeGateway.Tags;
using FluentResults;

namespace EdgeGateway;


public sealed class GatewayContext(IGateway gateway) : IGateway
{
    public required IPortManager Port { get; init; }
    public required IPortFactoryManager PortFactory { get; init; }
    public required IDeviceManager Device { get; init; }
    public required IDeviceFactoryManager DeviceFactory { get; init; }
    public required ITagReader TagReader { get; init; }
    public required ITagWriter TagWriter { get; init; }



    public bool IsRunning => gateway.IsRunning;
    public ValueTask<Result> StartAsync(CancellationToken cancellationToken = default) => gateway.StartAsync(cancellationToken);
    public ValueTask<Result> StopAsync(CancellationToken cancellationToken = default) => gateway.StopAsync(cancellationToken);
    public ValueTask DisposeAsync() => gateway.DisposeAsync();
}