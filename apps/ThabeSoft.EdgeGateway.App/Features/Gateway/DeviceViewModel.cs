using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class DeviceViewModel : LifecycleViewModel
{
    private readonly AvaloniaDictionary<SubscriptionId, SubscriptionViewModel> _subscription;
    private readonly IDevice _device;
    private readonly ISukiToastManager _sukiToastManager;

    public DeviceViewModel(IDevice device, ISukiToastManager sukiToastManager) : base(device, sukiToastManager)
    {
        _device = device;
        _sukiToastManager = sukiToastManager;

        _subscription = new(device.Subscriptions.Values
            .Select(x => KeyValuePair.Create(x.Id, CreateSubscriptionViewModel(x)))
            .ToDictionary());
    }

    public DeviceId Id => _device.Id;
    public ProtocolType ProtocolType => _device.ProtocolType;
    public IAvaloniaReadOnlyDictionary<SubscriptionId, SubscriptionViewModel> Subscriptions => _subscription;


    [RelayCommand]
    private async Task AddSubscription(SubscriptionDescriptor<int> subscriptionDescriptor)
    {
        var subscription = await _device.AddSubscriptionAsync(subscriptionDescriptor);
        _subscription.TryAdd(subscription.Id, CreateSubscriptionViewModel(subscription));
    }

    [RelayCommand]
    private void RemoveSubscription(SubscriptionId subscriptionId)
    {
        if (_subscription.TryGetValue(subscriptionId, out var vm))
            vm.Dispose();

        _subscription.Remove(subscriptionId);
    }


    private SubscriptionViewModel CreateSubscriptionViewModel(ISubscription subscription) =>
         new(subscription, _sukiToastManager);
}
