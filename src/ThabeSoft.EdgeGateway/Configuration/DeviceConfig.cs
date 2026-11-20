using System.Collections.Immutable;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Configuration;


public sealed record class DeviceConfig
{
    public required DeviceDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<SubscriptionConfig> Subscriptions { get; init => field = [.. value.WhereNotNull()]; } = [];
}
