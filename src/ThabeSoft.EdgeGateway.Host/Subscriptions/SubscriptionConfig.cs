using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscriptions;


public sealed record class SubscriptionConfig
{
    public required SubscriptionDescriptor Descriptor { get; init => field = value.Required(); }
    public required TagValueType ValueType { get; init => field = value.Required(); }
}