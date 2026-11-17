namespace ThabeSoft.EdgeGateway.Subscription;

internal interface ISubscriptionCreator
{
    Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull;
}