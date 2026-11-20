using System.Collections.Immutable;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Configuration.Models;


public sealed record class GatewayConfig
{
    public ImmutableArray<PortConfig> Ports { get; init => field = [.. value.WhereNotNull()]; } = [];
}
