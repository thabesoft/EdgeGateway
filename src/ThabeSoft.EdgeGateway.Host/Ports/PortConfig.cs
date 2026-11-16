using System.Collections.Immutable;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed record class PortConfig
{
    public required PortDescriptor Descriptor { get; init => field = value.Required(); }
    public ImmutableArray<DeviceConfig> Devices { get; init => field = [.. value.WhereNotNull()]; } = [];
}
