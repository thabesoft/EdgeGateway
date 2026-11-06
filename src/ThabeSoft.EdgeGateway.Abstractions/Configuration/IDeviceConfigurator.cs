using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;

public interface IDeviceConfigurator : IPortConfigurator
{
    ISubscriptionConfigurator<T> AddSubscription<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor) where T : notnull;
}
