using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface ITagOptionsConfigurator<T> where T : notnull
{
    ITagOptionsConfigurator<T> WithDescriptor(TagDescriptor<T> tagDescriptor);
    ITagOptionsConfigurator<T> WithTrigger(ITriggerDescriptor triggerDescriptor);
}