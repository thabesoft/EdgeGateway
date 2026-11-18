using System.Collections.Immutable;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed record class GatewayConfig
{
    public ImmutableArray<PortConfig> Ports { get; init => field = [.. value.WhereNotNull()]; } = [];
}
