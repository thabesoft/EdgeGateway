using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols;

namespace ThabeSoft.EdgeGateway.Readings;


/// <summary>
/// 数据接收端
/// </summary>
public interface IReadingSink
{
    ValueTask WriteAsync<T>(ReadingContext context, Reading<T> reading, CancellationToken cancellationToken = default) where T : notnull;
}


public readonly record struct ReadingContext
{
    public required PortId PortId { get; init => field = value.Required(); }
    public required DeviceId DeviceId { get; init => field = value.Required(); }
    public required ProtocolAddress DataAddress { get; init => field = value.Required(); }
}