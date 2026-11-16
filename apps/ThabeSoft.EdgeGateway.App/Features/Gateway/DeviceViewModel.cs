using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class DeviceViewModel : ObservableObject, IDisposable
{
    private readonly AvaloniaDictionary<SubscriptionId, SubscriptionViewModel> _viewModels;
    private readonly IDevice _device;
    private readonly ISukiToastManager _sukiToastManager;
    private readonly IDisposable[] _eventDisposers;


    public DeviceViewModel(IDevice device, IEventSubscriber eventSubscriber, ISukiToastManager sukiToastManager)
    {
        _device = device;
        _sukiToastManager = sukiToastManager;
        _viewModels = new(device.Subscriptions.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));

        _eventDisposers =
        [
             eventSubscriber.Subscribe<SubscriptionAddedEvent>((e, _) =>
            {
                if(e.DeviceId != device.Id)return ValueTask.CompletedTask;

                if(device.Subscriptions.TryGetValue(e.SubscriptionId, out var subscription))
                        _viewModels[e.SubscriptionId] = CreateViewModel(subscription);

                return ValueTask.CompletedTask;
            }),
            eventSubscriber.Subscribe<SubscriptionRemovedEvent>((e, _) =>
            {
                if(e.DeviceId != device.Id) return ValueTask.CompletedTask;

                _viewModels.Remove(e.SubscriptionId);
                return ValueTask.CompletedTask;
            }),
            eventSubscriber.Subscribe<DeviceStateChangedEvent>((e, _) =>
            {
                if(e.DeviceId != device.Id) return ValueTask.CompletedTask;
                
                State = e.New;
                return ValueTask.CompletedTask;
            })
        ];

        SubscriptionViewModel CreateViewModel(ISubscription subscription) => new(subscription, _sukiToastManager);
    }

    public DeviceId Id => _device.Id;
    public ProtocolType ProtocolType => _device.ProtocolType;
    public IAvaloniaReadOnlyDictionary<SubscriptionId, SubscriptionViewModel> Subscriptions => _viewModels;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    public partial LifecycleState State { get; private set; }
    public bool IsRunning => State == LifecycleState.Running;


    [RelayCommand]
    private async Task StartAsync()
    {
        await _sukiToastManager.ExecuteWithToastAsync(_device.StartAsync, "启动设备");
    }
    [RelayCommand]
    private async Task StopAsync()
    {
        await _sukiToastManager.ExecuteWithToastAsync(_device.StopAsync, "停止设备");
    }
    [RelayCommand]
    private async Task RemoveSubscriptionAsync(SubscriptionId id)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
            await _device.RemoveSubscriptionAsync(id, ct), "删除订阅");
    }


    public void Dispose()
    {
        foreach (var i in _eventDisposers)
            i.Dispose();
    }
}