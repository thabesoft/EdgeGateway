using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Readings;


internal sealed class ReadingSinkBus(IEnumerable<IReadingSink> readingSinks)
{
    public async ValueTask PublishAsync<T>(Subscription<T> subscription, Reading<T> reading, CancellationToken cancellationToken = default)
        where T : notnull
    {
        foreach(var i in readingSinks)
        {
           await i.WriteAsync(reading, cancellationToken);
        }
    }
}

internal sealed record class A
{
    public required ConnectionEndpoint Endpoint { get; init; }
    public required DeviceId DeviceId { get; init; }
    public required SubscriptionId SubscriptionId { get; init; }
}