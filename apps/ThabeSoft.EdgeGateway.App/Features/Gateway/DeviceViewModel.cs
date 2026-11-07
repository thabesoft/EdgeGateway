using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class DeviceViewModel(IDevice device, ISukiToastManager sukiToastManager) : LifecycleViewModel(device, sukiToastManager), IAsyncDisposable
{
    private readonly AvaloniaDictionary<SubscriptionId, SubscriptionViewModeel> _subscription = new(
        device.Subscriptions.Values
            .Select(x => KeyValuePair.Create(x.Id, new SubscriptionViewModeel(x, sukiToastManager)))
            .ToDictionary()
    );


    public DeviceId Id => device.Id;
    public ProtocolType ProtocolType => device.ProtocolType;
    public IAvaloniaReadOnlyDictionary<SubscriptionId, SubscriptionViewModeel> Subscriptions => _subscription;


    [RelayCommand]
    private void AddSubscription(SubscriptionDescriptor<int> subscriptionDescriptor)
    {
        var subscription = device.AddSubscription(subscriptionDescriptor);
        _subscription[subscription.Id] = new SubscriptionViewModeel(subscription, sukiToastManager);
    }

    public ValueTask DisposeAsync() => device.DisposeAsync();
}
