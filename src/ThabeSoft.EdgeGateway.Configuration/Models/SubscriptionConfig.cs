using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.Configuration.Models;


public sealed record class SubscriptionConfig
{
    public required ISubscriptionDescriptor Descriptor { get; init => field = value.Required(); }
}