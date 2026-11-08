namespace ThabeSoft.EdgeGateway.Triggers;

public interface ITriggerFactorySelector
{
    ITriggerFactory Select(ITriggerDescriptor triggerDescriptor);
}

public static class TriggerFactorySelectorExtensions
{
    extension(ITriggerFactorySelector factorySelector)
    {
        public ITrigger Create(ITriggerDescriptor triggerDescriptor)
        {
            return factorySelector.Select(triggerDescriptor).Create(triggerDescriptor);
        }
    }
}