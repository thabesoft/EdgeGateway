using System.Collections.Immutable;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Configuration;


public sealed record class PortConfig
{
    public required PortDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<DeviceConfig> Devices { get; init => field = [.. value.WhereNotNull()]; } = [];
}
