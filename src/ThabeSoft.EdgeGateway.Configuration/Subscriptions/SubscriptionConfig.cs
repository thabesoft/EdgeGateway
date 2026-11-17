using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscriptions;


public sealed record class SubscriptionConfig
{
    public SubscriptionId Id { get; init => field = value.Required(); } = SubscriptionId.Create();
    public required TriggerDescriptor Trigger { get; init => field = value.Required(); }
    public required DataAddress Address { get; init => field = value.Required(); }
    public required TagValueType ValueType { get; init => field = value.Required(); }
}