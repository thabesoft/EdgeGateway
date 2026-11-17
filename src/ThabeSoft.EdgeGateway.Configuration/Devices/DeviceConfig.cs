using System.Collections.Immutable;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscriptions;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed record class DeviceConfig
{
    public DeviceId Id { get; init => field = value.Required(); } = DeviceId.Create();
    public required ProtocolDescriptor Protocol { get; init => field = value.Required(); }
    public ImmutableArray<SubscriptionConfig> Subscriptions { get; init => field = [.. value.WhereNotNull()]; } = [];
}
