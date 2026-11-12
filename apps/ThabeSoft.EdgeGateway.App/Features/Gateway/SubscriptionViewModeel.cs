using CommunityToolkit.Mvvm.ComponentModel;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class SubscriptionViewModel : LifecycleViewModel, IDisposable
{
    private readonly ISubscription _subscription;
    private readonly IDisposable _subscriptionHandle;


    public SubscriptionViewModel(ISubscription subscription, ISukiToastManager sukiToastManager) : base(subscription, sukiToastManager)
    {
        _subscription = subscription;
        Latest = subscription.Latest;
        _subscriptionHandle = subscription.Subscribe(
            onNext: value => Latest = value,
            onError: _ => Latest = Reading.Bad(Latest?.Value ?? default!));
    }

    public SubscriptionId Id => _subscription.Id;
    public IDataAddress Address => _subscription.Address;
    [ObservableProperty] public partial Reading? Latest { get; set; }


    public void Dispose()
    {
        _subscriptionHandle.Dispose();
    }
}