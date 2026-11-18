using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed record class SubscriptionConfig
{
    public required ISubscriptionDescriptor Descriptor { get; init => field = value.Required(); }
}