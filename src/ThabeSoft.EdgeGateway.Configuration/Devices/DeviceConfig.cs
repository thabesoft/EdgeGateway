using System.Collections.Immutable;
using ThabeSoft.EdgeGateway.Subscriptions;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed record class DeviceConfig
{
    public required DeviceDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<SubscriptionConfig> Subscriptions { get; init => field = [.. value.WhereNotNull()]; } = [];
}
