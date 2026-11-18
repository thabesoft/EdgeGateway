using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.DataTypes;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Models;


public sealed record class SubscriptionConfig
{
    public required SubscriptionDescriptor Descriptor { get; init => field = value.Required(); }
    public required TriggerDescriptor Trigger { get; init => field = value.Required(); }
    public required DataType ValueType { get; init => field = value.Defined(); }
}