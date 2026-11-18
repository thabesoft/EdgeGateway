using System.Collections.Immutable;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed record class DeviceConfig
{
    public DeviceDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<SubscriptionConfig> Subscriptions { get; init => field = [.. value.WhereNotNull()]; } = [];
}
