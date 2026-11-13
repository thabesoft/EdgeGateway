using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class DeviceViewModel : LifecycleViewModel
{
    private readonly AvaloniaDictionary<SubscriptionId, SubscriptionViewModel> _viewModels;
    private readonly IDevice _device;
    private readonly ISukiToastManager _sukiToastManager;

    public DeviceViewModel(IDevice device, ISukiToastManager sukiToastManager) : base(device, sukiToastManager)
    {
        _device = device;
        _sukiToastManager = sukiToastManager;
        _viewModels = new(device.Subscriptions.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));
    }

    public DeviceId Id => _device.Id;
    public ProtocolType ProtocolType => _device.ProtocolType;
    public IAvaloniaReadOnlyDictionary<SubscriptionId, SubscriptionViewModel> Subscriptions => _viewModels;


    [RelayCommand]
    private async Task AddDoubleSubscription(SubscriptionDescriptor<double> subscriptionDescriptor)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
            await AddSubscriptionAsync(subscriptionDescriptor, ct), "添加设备", TimeSpan.FromSeconds(30));
    }

    [RelayCommand]
    private async Task RemoveSubscriptionAsync(SubscriptionId subscriptionId)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
          await RemoveSubscriptionAsync(subscriptionId, ct), "删除设备", TimeSpan.FromSeconds(10));
    }

    public override async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        await base.StartAsync(cancellationToken);

        foreach (var i in _viewModels.Values)
            i.OnUpdateState();
    }
    public override async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        await base.StopAsync(cancellationToken);

        foreach (var i in _viewModels.Values)
            i.OnUpdateState();
    }

    public async ValueTask<SubscriptionViewModel> AddSubscriptionAsync<T>(SubscriptionDescriptor<T> subscriptionDescriptor, CancellationToken cancellationToken = default)
        where T : notnull
    {
        var model = await _device.AddSubscriptionAsync(subscriptionDescriptor, cancellationToken);
        var viewModel = CreateViewModel(model);

        _viewModels[subscriptionDescriptor.Id] = viewModel;
        return viewModel;
    }
    public async ValueTask<bool> RemoveSubscriptionAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default)
    {
        var result = await _device.RemoveSubscriptionAsync(subscriptionId);
        _viewModels.Remove(subscriptionId);

        return result;
    }


    private SubscriptionViewModel CreateViewModel(ISubscription subscription) => new(subscription, _sukiToastManager);
}
