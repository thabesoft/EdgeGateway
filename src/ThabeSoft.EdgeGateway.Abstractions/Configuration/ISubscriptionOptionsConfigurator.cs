using ThabeSoft.EdgeGateway.Polling;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface ISubscriptionOptionsConfigurator<T> where T : notnull
{
    ISubscriptionOptionsConfigurator<T> WithDescriptor(SubscriptionDescriptor<T> tagDescriptor);
    ISubscriptionOptionsConfigurator<T> AddTagCallback(Action<T> tagDescriptor);
}